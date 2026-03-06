using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Chatbot.API.Data.Context;
using Chatbot.API.Services.Core;
using Chatbot.Core.Models;
using Chatbot.Core.Models.Entities;
using Chatbot.API.Exceptions;
using Chatbot.API.Infrastructure.Auth;
using Chatbot.API.Infrastructure.Authorization;
using Chatbot.API.Infrastructure.Facades;
using Chatbot.API.Infrastructure.Http;
using System.IdentityModel.Tokens.Jwt;

namespace Chatbot.API.Controllers.Chat;

/// <summary>
/// Chat API controller with reduced dependencies via facade pattern.
/// Implements single responsibility - delegates to specialized services via facade.
/// Uses base controller class to reduce code duplication.
/// </summary>
[Authorize]
public class ChatController : ApiControllerBase
{
    private readonly IAuthenticationService _authService;
    private readonly IChatFacadeService _chatFacade;
    private readonly IConversationAccessControl _accessControl;
    private readonly ChatbotDbContext _db;

    public ChatController(
        IAuthenticationService authService,
        IChatFacadeService chatFacade,
        IConversationAccessControl accessControl,
        ChatbotDbContext db,
        IUserContextProvider userContextProvider,
        IApiResponseBuilder responseBuilder,
        ILogger<ChatController> logger)
        : base(userContextProvider, responseBuilder, logger)
    {
        _authService = authService;
        _chatFacade = chatFacade;
        _accessControl = accessControl;
        _db = db;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] CreateUserRequest request)
    {
        LogAction("Register", new { username = request.Username });

        var (success, token, message, user) = await _authService.RegisterAsync(request.Username, request.Email, request.Password);
        if (!success || user == null)
            throw new ConflictException(message);

        var response = new AuthResponse(token, user.Username, user.Email, DateTime.UtcNow.AddMinutes(1440));
        return Ok(response, message);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        LogAction("Login", new { username = request.Username });

        var (success, token, message, user) = await _authService.LoginAsync(request.Username, request.Password);
        if (!success || user == null)
            throw new UnauthorizedException(message);

        var response = new AuthResponse(token, user.Username, user.Email, DateTime.UtcNow.AddMinutes(1440));
        return Ok(response, message);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        LogAction("Logout");

        var authHeader = Request.Headers.Authorization.FirstOrDefault();
        if (authHeader != null && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            var token = authHeader["Bearer ".Length..].Trim();
            var handler = new JwtSecurityTokenHandler();
            if (handler.CanReadToken(token))
            {
                var jwt = handler.ReadJwtToken(token);
                var jti = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
                if (jti != null)
                {
                    _db.RevokedTokens.Add(new RevokedToken
                    {
                        Jti = jti,
                        RevokedAt = DateTime.UtcNow,
                        ExpiresAt = jwt.ValidTo
                    });

                    // Clean up expired revocations opportunistically
                    var expired = await _db.RevokedTokens
                        .Where(r => r.ExpiresAt < DateTime.UtcNow)
                        .ToListAsync();
                    _db.RevokedTokens.RemoveRange(expired);

                    await _db.SaveChangesAsync();
                }
            }
        }

        return Ok<object>(null!, "Logged out successfully");
    }

    [HttpPost("conversations")]
    public async Task<IActionResult> StartConversation([FromBody] StartConversationRequest request)
    {
        var userId = CurrentUserId;
        LogAction("CreateConversation", new { Title = request.Title });

        var conversation = await _chatFacade.CreateConversationAsync(userId, request.Title);
        var response = new ConversationResponse(
            conversation.Id,
            conversation.Title,
            conversation.StartedAt,
            0,
            conversation.Summary);

        return Ok(response, "Conversation started");
    }

    [HttpGet("conversations")]
    public async Task<IActionResult> GetConversations([FromQuery] string? search = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var userId = CurrentUserId;
        LogAction("GetConversations", new { search, page, pageSize });

        var query = _db.Conversations
            .AsNoTracking()
            .Where(c => c.UserId == userId && c.IsActive);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(c => EF.Functions.Like(c.Title!, $"%{search}%"));

        var total = await query.CountAsync();
        var conversations = await query
            .OrderByDescending(c => c.LastMessageAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new ConversationResponse(
                c.Id,
                c.Title,
                c.StartedAt,
                c.Messages.Count,
                c.Summary))
            .ToListAsync();

        return Ok(new PaginatedResponse<ConversationResponse>(total, page, pageSize, conversations), "Conversations retrieved");
    }

    [HttpPost("{conversationId}/send")]
    public async Task<IActionResult> SendMessage(int conversationId, [FromBody] ChatMessageRequest request)
    {
        var userId = CurrentUserId;
        LogAction("SendMessage", new { conversationId, messageLength = request.Message.Length });

        await _accessControl.VerifyAccessAsync(conversationId, userId);
        var response = await _chatFacade.SendMessageAsync(conversationId, request.Message);

        return Ok(response, "Message processed");
    }

    [HttpGet("{conversationId}/history")]
    public async Task<IActionResult> GetHistory(int conversationId, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        var userId = CurrentUserId;
        LogAction("GetHistory", new { conversationId, page, pageSize });

        await _accessControl.VerifyAccessAsync(conversationId, userId);

        var total = await _db.Messages.CountAsync(m => m.ConversationId == conversationId);
        var messages = await _db.Messages
            .AsNoTracking()
            .Where(m => m.ConversationId == conversationId)
            .OrderBy(m => m.SentAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(m => new MessageDto(
                m.Id,
                m.Content,
                m.Sender.ToString(),
                m.SentAt,
                m.Sentiment.ToString(),
                m.DetectedIntent,
                m.SentimentScore))
            .ToListAsync();

        var response = new
        {
            ConversationId = conversationId,
            Total = total,
            Page = page,
            PageSize = pageSize,
            Messages = messages
        };

        return Ok(response, "History retrieved");
    }

    [HttpPost("{conversationId}/messages/{messageId}/feedback")]
    public async Task<IActionResult> SubmitFeedback(int conversationId, int messageId, [FromBody] MessageFeedbackRequest request)
    {
        var userId = CurrentUserId;
        LogAction("SubmitFeedback", new { conversationId, messageId, rating = request.Rating });

        await _accessControl.VerifyAccessAsync(conversationId, userId);

        var message = await _db.Messages.FindAsync(messageId);
        if (message == null || message.ConversationId != conversationId)
            throw new NotFoundException("Message not found");

        // Upsert feedback (one per user per message)
        var existing = await _db.MessageFeedback
            .FirstOrDefaultAsync(f => f.MessageId == messageId && f.UserId == userId);

        if (existing != null)
        {
            existing.Rating = request.Rating;
            existing.Comment = request.Comment;
            existing.CreatedAt = DateTime.UtcNow;
        }
        else
        {
            _db.MessageFeedback.Add(new MessageFeedback
            {
                MessageId = messageId,
                UserId = userId,
                Rating = request.Rating,
                Comment = request.Comment,
                Message = null!,
                User = null!
            });
        }

        await _db.SaveChangesAsync();
        return Ok<object>(null!, "Feedback submitted");
    }

    [HttpPost("{conversationId}/escalate")]
    public async Task<IActionResult> EscalateConversation(int conversationId, [FromBody] EscalationRequest request)
    {
        var userId = CurrentUserId;
        LogAction("EscalateConversation", new { conversationId });

        await _accessControl.VerifyAccessAsync(conversationId, userId);

        var conversation = await _db.Conversations.FindAsync(conversationId);
        if (conversation == null) throw new NotFoundException("Conversation not found");

        _db.EscalationTickets.Add(new EscalationTicket
        {
            ConversationId = conversationId,
            UserId = userId,
            Reason = request.Reason,
            Status = TicketStatus.Open,
            Conversation = null!,
            User = null!
        });

        await _db.SaveChangesAsync();
        return Ok<object>(null!, "Escalation ticket created. A human agent will review your conversation.");
    }

    [HttpGet("health")]
    [AllowAnonymous]
    public IActionResult Health()
    {
        return base.Ok(new HealthResponse("healthy", DateTime.UtcNow, "1.0.0"));
    }

    [HttpGet("analytics")]
    public async Task<IActionResult> GetAnalytics([FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
    {
        var userId = CurrentUserId;
        LogAction("GetAnalytics", new { startDate, endDate });

        var analytics = await _chatFacade.GetAnalyticsAsync(userId, startDate, endDate);
        return Ok(analytics, "Analytics retrieved successfully");
    }

    [HttpGet("analytics/sentiment-trends")]
    public async Task<IActionResult> GetSentimentTrends([FromQuery] int days = 7)
    {
        var userId = CurrentUserId;
        LogAction("GetSentimentTrends", new { days });

        var trends = await _chatFacade.GetSentimentTrendsAsync(userId, days);
        return Ok(trends, "Sentiment trends retrieved successfully");
    }

    [HttpGet("analytics/intent-distribution")]
    public async Task<IActionResult> GetIntentDistribution([FromQuery] int days = 30)
    {
        var userId = CurrentUserId;
        LogAction("GetIntentDistribution", new { days });

        var distribution = await _chatFacade.GetIntentDistributionAsync(userId, days);
        return Ok(distribution, "Intent distribution retrieved successfully");
    }

    [HttpGet("analytics/satisfaction")]
    public async Task<IActionResult> GetSatisfactionRate([FromQuery] int days = 30)
    {
        var userId = CurrentUserId;
        LogAction("GetSatisfactionRate", new { days });

        var since = DateTime.UtcNow.AddDays(-days);
        var feedback = await _db.MessageFeedback
            .AsNoTracking()
            .Include(f => f.Message)
            .ThenInclude(m => m.Conversation)
            .Where(f => f.Message.Conversation.UserId == userId && f.CreatedAt >= since)
            .ToListAsync();

        var total = feedback.Count;
        var positive = feedback.Count(f => f.Rating == FeedbackRating.Positive);
        var rate = total > 0 ? (double)positive / total * 100 : 0;

        return Ok(new { Total = total, Positive = positive, SatisfactionRate = Math.Round(rate, 1) }, "Satisfaction rate retrieved");
    }

    [HttpGet("preferences")]
    public async Task<IActionResult> GetPreferences()
    {
        var userId = CurrentUserId;
        LogAction("GetPreferences");

        var preferences = await _chatFacade.GetPreferencesAsync(userId);
        return Ok(preferences, "Preferences retrieved successfully");
    }

    [HttpPut("preferences")]
    public async Task<IActionResult> UpdatePreferences([FromBody] UserPreferences preferences)
    {
        var userId = CurrentUserId;
        LogAction("UpdatePreferences");

        var updated = await _chatFacade.UpdatePreferencesAsync(userId, preferences);
        return Ok(updated, "Preferences updated successfully");
    }

    [HttpGet("{id}/export/json")]
    public async Task<IActionResult> ExportConversationJson(int id)
    {
        var userId = CurrentUserId;
        LogAction("ExportConversationJson", new { conversationId = id });

        await _accessControl.VerifyAccessAsync(id, userId);
        var bytes = await _chatFacade.ExportConversationJsonAsync(id);

        return File(bytes, "application/json", $"conversation_{id}_{DateTime.UtcNow:yyyyMMdd}.json");
    }

    [HttpGet("{id}/export/csv")]
    public async Task<IActionResult> ExportConversationCsv(int id)
    {
        var userId = CurrentUserId;
        LogAction("ExportConversationCsv", new { conversationId = id });

        await _accessControl.VerifyAccessAsync(id, userId);
        var bytes = await _chatFacade.ExportConversationCsvAsync(id);

        return File(bytes, "text/csv", $"conversation_{id}_{DateTime.UtcNow:yyyyMMdd}.csv");
    }
}

public record MessageFeedbackRequest(FeedbackRating Rating, string? Comment = null);
public record EscalationRequest(string Reason);
