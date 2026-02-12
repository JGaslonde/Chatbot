using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Chatbot.API.Services;
using Chatbot.Core.Models;
using Chatbot.Core.Models.Entities;
using Chatbot.API.Exceptions;

namespace Chatbot.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Protect all endpoints by default
public class ChatController : ControllerBase
{
    private readonly IConversationService _conversationService;
    private readonly IAuthenticationService _authService;
    private readonly IConversationAnalyticsService _analyticsService;
    private readonly IUserPreferencesService _preferencesService;
    private readonly IConversationExportService _exportService;
    private readonly ILogger<ChatController> _logger;

    public ChatController(
        IConversationService conversationService,
        IAuthenticationService authService,
        IConversationAnalyticsService analyticsService,
        IUserPreferencesService preferencesService,
        IConversationExportService exportService,
        ILogger<ChatController> logger)
    {
        _conversationService = conversationService;
        _authService = authService;
        _analyticsService = analyticsService;
        _preferencesService = preferencesService;
        _exportService = exportService;
        _logger = logger;
    }

    [HttpPost("register")]
    [AllowAnonymous] // Allow unauthenticated access for registration
    public async Task<IActionResult> Register([FromBody] CreateUserRequest request)
    {
        var (success, token, message, user) = await _authService.RegisterAsync(request.Username, request.Email, request.Password);
        if (!success || user == null)
            throw new ConflictException(message);

        return Ok(new ApiResponse<AuthResponse>(true, message, 
            new AuthResponse(token, user.Username, user.Email, DateTime.UtcNow.AddMinutes(1440))));
    }

    [HttpPost("login")]
    [AllowAnonymous] // Allow unauthenticated access for login
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var (success, token, message, user) = await _authService.LoginAsync(request.Username, request.Password);
        if (!success || user == null)
            throw new UnauthorizedException(message);

        return Ok(new ApiResponse<AuthResponse>(true, message, 
            new AuthResponse(token, user.Username, user.Email, DateTime.UtcNow.AddMinutes(1440))));
    }

    [HttpPost("conversations")]
    public async Task<IActionResult> StartConversation([FromBody] StartConversationRequest request)
    {
        // Extract user ID from JWT token
        var userIdClaim = User.FindFirst("id");
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            throw new UnauthorizedException("Invalid token");

        var conversation = await _conversationService.CreateConversationAsync(userId, request.Title);

        var response = new ConversationResponse(
            conversation.Id,
            conversation.Title,
            conversation.StartedAt,
            0,
            conversation.Summary);

        return Ok(new ApiResponse<ConversationResponse>(true, "Conversation started", response));
    }

    [HttpPost("send")]
    [Route("{conversationId}/send")]
    public async Task<IActionResult> SendMessage(int conversationId, [FromBody] ChatMessageRequest request)
    {
        // Add user message
        var message = await _conversationService.AddMessageAsync(conversationId, request.Message, MessageSender.User);

        // Generate intelligent bot response using new template service
        var botResponseText = await _conversationService.GenerateBotResponseAsync(conversationId, request.Message);
        var botMessage = await _conversationService.AddMessageAsync(conversationId, botResponseText, MessageSender.Bot);

        // Update conversation summary periodically (every 5th message)
        if (conversationId % 5 == 0)
        {
            await _conversationService.UpdateConversationSummaryAsync(conversationId);
        }

        var response = new ChatMessageResponse(
            botMessage.Content,
            botMessage.SentAt,
            botMessage.DetectedIntent ?? "unknown",
            botMessage.IntentConfidence,
            botMessage.Sentiment.ToString(),
            botMessage.SentimentScore,
            conversationId);

        return Ok(new ApiResponse<ChatMessageResponse>(true, "Message processed", response));
    }

    [HttpGet("{conversationId}/history")]
    public async Task<IActionResult> GetHistory(int conversationId)
    {
        var conversation = await _conversationService.GetConversationAsync(conversationId);
        if (conversation == null)
            throw new NotFoundException("Conversation", conversationId);

        var messageDtos = conversation.Messages
            .OrderBy(m => m.SentAt)
            .Select(m => new MessageDto(
                m.Id,
                m.Content,
                m.Sender.ToString(),
                m.SentAt,
                m.Sentiment.ToString(),
                m.DetectedIntent,
                m.SentimentScore))
            .ToList();

        var response = new MessageHistoryResponse(conversationId, messageDtos);
        return Ok(new ApiResponse<MessageHistoryResponse>(true, "History retrieved", response));
    }

    [HttpGet("health")]
    [AllowAnonymous] // Allow unauthenticated access for health checks
    public IActionResult Health()
    {
        return Ok(new HealthResponse("healthy", DateTime.UtcNow, "1.0.0"));
    }

    [HttpGet("analytics")]
    public async Task<IActionResult> GetAnalytics([FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
    {
        var userIdClaim = User.FindFirst("id")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            throw new UnauthorizedException("Invalid user token");

        var analytics = await _analyticsService.GetAnalyticsAsync(userId, startDate, endDate);
        return Ok(new ApiResponse<ConversationAnalytics>(true, "Analytics retrieved successfully", analytics));
    }

    [HttpGet("analytics/sentiment-trends")]
    public async Task<IActionResult> GetSentimentTrends([FromQuery] int days = 7)
    {
        var userIdClaim = User.FindFirst("id")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            throw new UnauthorizedException("Invalid user token");

        var trends = await _analyticsService.GetSentimentTrendsAsync(userId, days);
        return Ok(new ApiResponse<List<SentimentTrend>>(true, "Sentiment trends retrieved successfully", trends));
    }

    [HttpGet("analytics/intent-distribution")]
    public async Task<IActionResult> GetIntentDistribution([FromQuery] int days = 30)
    {
        var userIdClaim = User.FindFirst("id")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            throw new UnauthorizedException("Invalid user token");

        var distribution = await _analyticsService.GetIntentDistributionAsync(userId, days);
        return Ok(new ApiResponse<List<IntentDistribution>>(true, "Intent distribution retrieved successfully", distribution));
    }

    [HttpGet("preferences")]
    public async Task<IActionResult> GetPreferences()
    {
        var userIdClaim = User.FindFirst("id")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            throw new UnauthorizedException("Invalid user token");

        var preferences = await _preferencesService.GetPreferencesAsync(userId);
        return Ok(new ApiResponse<UserPreferences>(true, "Preferences retrieved successfully", preferences));
    }

    [HttpPut("preferences")]
    public async Task<IActionResult> UpdatePreferences([FromBody] UserPreferences preferences)
    {
        var userIdClaim = User.FindFirst("id")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            throw new UnauthorizedException("Invalid user token");

        var updated = await _preferencesService.UpdatePreferencesAsync(userId, preferences);
        return Ok(new ApiResponse<UserPreferences>(true, "Preferences updated successfully", updated));
    }

    [HttpGet("{id}/export/json")]
    public async Task<IActionResult> ExportConversationJson(int id)
    {
        var userIdClaim = User.FindFirst("id")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            throw new UnauthorizedException("Invalid user token");

        // Verify conversation exists and user owns it
        var conversation = await _conversationService.GetConversationAsync(id);
        if (conversation == null)
            throw new NotFoundException($"Conversation {id} not found");
        
        if (conversation.UserId != userId)
            throw new UnauthorizedException("Access denied to this conversation");

        var bytes = await _exportService.ExportToJsonBytesAsync(id);
        return File(bytes, "application/json", $"conversation_{id}_{DateTime.UtcNow:yyyyMMdd}.json");
    }

    [HttpGet("{id}/export/csv")]
    public async Task<IActionResult> ExportConversationCsv(int id)
    {
        var userIdClaim = User.FindFirst("id")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            throw new UnauthorizedException("Invalid user token");

        // Verify conversation exists and user owns it
        var conversation = await _conversationService.GetConversationAsync(id);
        if (conversation == null)
            throw new NotFoundException($"Conversation {id} not found");
        
        if (conversation.UserId != userId)
            throw new UnauthorizedException("Access denied to this conversation");

        var bytes = await _exportService.ExportToCsvBytesAsync(id);
        return File(bytes, "text/csv", $"conversation_{id}_{DateTime.UtcNow:yyyyMMdd}.csv");
    }
}
