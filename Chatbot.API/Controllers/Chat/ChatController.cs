using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Chatbot.API.Services.Core;
using Chatbot.Core.Models;
using Chatbot.Core.Models.Entities;
using Chatbot.API.Exceptions;
using Chatbot.API.Infrastructure.Auth;
using Chatbot.API.Infrastructure.Authorization;
using Chatbot.API.Infrastructure.Facades;
using Chatbot.API.Infrastructure.Http;

namespace Chatbot.API.Controllers.Chat;

/// <summary>
/// Chat API controller with reduced dependencies via facade pattern.
/// Implements single responsibility - delegates to specialized services via facade.
/// Uses base controller class to reduce code duplication.
/// </summary>
[AllowAnonymous] // Override in specific endpoints
public class ChatController : ApiControllerBase
{
    private readonly IAuthenticationService _authService;
    private readonly IChatFacadeService _chatFacade;
    private readonly IConversationAccessControl _accessControl;

    public ChatController(
        IAuthenticationService authService,
        IChatFacadeService chatFacade,
        IConversationAccessControl accessControl,
        IUserContextProvider userContextProvider,
        IApiResponseBuilder responseBuilder,
        ILogger<ChatController> logger)
        : base(userContextProvider, responseBuilder, logger)
    {
        _authService = authService;
        _chatFacade = chatFacade;
        _accessControl = accessControl;
    }

    [HttpPost("register")]
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
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        LogAction("Login", new { username = request.Username });

        var (success, token, message, user) = await _authService.LoginAsync(request.Username, request.Password);
        if (!success || user == null)
            throw new UnauthorizedException(message);

        var response = new AuthResponse(token, user.Username, user.Email, DateTime.UtcNow.AddMinutes(1440));
        return Ok(response, message);
    }

    [HttpPost("conversations")]
    [Authorize]
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

    [HttpPost("send")]
    [Route("{conversationId}/send")]
    [Authorize]
    public async Task<IActionResult> SendMessage(int conversationId, [FromBody] ChatMessageRequest request)
    {
        var userId = CurrentUserId;
        LogAction("SendMessage", new { conversationId, messageLength = request.Message.Length });

        await _accessControl.VerifyAccessAsync(conversationId, userId);
        var response = await _chatFacade.SendMessageAsync(conversationId, request.Message);

        return Ok(response, "Message processed");
    }

    [HttpGet("{conversationId}/history")]
    [Authorize]
    public async Task<IActionResult> GetHistory(int conversationId)
    {
        var userId = CurrentUserId;
        LogAction("GetHistory", new { conversationId });

        await _accessControl.VerifyAccessAsync(conversationId, userId);
        var response = await _chatFacade.GetConversationHistoryAsync(conversationId);

        return Ok(response, "History retrieved");
    }

    [HttpGet("health")]
    [AllowAnonymous]
    public IActionResult Health()
    {
        return base.Ok(new HealthResponse("healthy", DateTime.UtcNow, "1.0.0"));
    }

    [HttpGet("analytics")]
    [Authorize]
    public async Task<IActionResult> GetAnalytics([FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
    {
        var userId = CurrentUserId;
        LogAction("GetAnalytics", new { startDate, endDate });

        var analytics = await _chatFacade.GetAnalyticsAsync(userId, startDate, endDate);
        return Ok(analytics, "Analytics retrieved successfully");
    }

    [HttpGet("analytics/sentiment-trends")]
    [Authorize]
    public async Task<IActionResult> GetSentimentTrends([FromQuery] int days = 7)
    {
        var userId = CurrentUserId;
        LogAction("GetSentimentTrends", new { days });

        var trends = await _chatFacade.GetSentimentTrendsAsync(userId, days);
        return Ok(trends, "Sentiment trends retrieved successfully");
    }

    [HttpGet("analytics/intent-distribution")]
    [Authorize]
    public async Task<IActionResult> GetIntentDistribution([FromQuery] int days = 30)
    {
        var userId = CurrentUserId;
        LogAction("GetIntentDistribution", new { days });

        var distribution = await _chatFacade.GetIntentDistributionAsync(userId, days);
        return Ok(distribution, "Intent distribution retrieved successfully");
    }

    [HttpGet("preferences")]
    [Authorize]
    public async Task<IActionResult> GetPreferences()
    {
        var userId = CurrentUserId;
        LogAction("GetPreferences");

        var preferences = await _chatFacade.GetPreferencesAsync(userId);
        return Ok(preferences, "Preferences retrieved successfully");
    }

    [HttpPut("preferences")]
    [Authorize]
    public async Task<IActionResult> UpdatePreferences([FromBody] UserPreferences preferences)
    {
        var userId = CurrentUserId;
        LogAction("UpdatePreferences");

        var updated = await _chatFacade.UpdatePreferencesAsync(userId, preferences);
        return Ok(updated, "Preferences updated successfully");
    }

    [HttpGet("{id}/export/json")]
    [Authorize]
    public async Task<IActionResult> ExportConversationJson(int id)
    {
        var userId = CurrentUserId;
        LogAction("ExportConversationJson", new { conversationId = id });

        await _accessControl.VerifyAccessAsync(id, userId);
        var bytes = await _chatFacade.ExportConversationJsonAsync(id);

        return File(bytes, "application/json", $"conversation_{id}_{DateTime.UtcNow:yyyyMMdd}.json");
    }

    [HttpGet("{id}/export/csv")]
    [Authorize]
    public async Task<IActionResult> ExportConversationCsv(int id)
    {
        var userId = CurrentUserId;
        LogAction("ExportConversationCsv", new { conversationId = id });

        await _accessControl.VerifyAccessAsync(id, userId);
        var bytes = await _chatFacade.ExportConversationCsvAsync(id);

        return File(bytes, "text/csv", $"conversation_{id}_{DateTime.UtcNow:yyyyMMdd}.csv");
    }
}
