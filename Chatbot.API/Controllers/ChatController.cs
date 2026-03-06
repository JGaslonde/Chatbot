using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Chatbot.API.Services;
using Chatbot.Core.Models;
using Chatbot.Core.Models.Entities;
using Chatbot.Core.Models.Requests;
using Chatbot.Core.Models.Responses;
using Chatbot.API.Exceptions;
using Chatbot.API.Infrastructure;
using Chatbot.API.Services.Analysis.Interfaces;
using Chatbot.API.Services.Analytics.Interfaces;
using Chatbot.API.Services.Core.Interfaces;
using Chatbot.API.Services.Export.Interfaces;
using Chatbot.API.Controllers.Chat;
using Chatbot.API.Infrastructure.Facades.Interfaces;
using Chatbot.API.Infrastructure.Authorization.Interfaces;
using Chatbot.API.Infrastructure.Auth.Interfaces;
using Chatbot.API.Infrastructure.Http.Interfaces;

namespace Chatbot.API.Controllers;

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
    private readonly IAdvancedSearchService _advancedSearch;
    private readonly IAdvancedAnalyticsService _advancedAnalytics;
    private readonly IAuditLoggingService _auditLogging;
    private readonly IExportService _exportService;
    private readonly IBatchOperationService _batchOperation;
    private readonly INotificationService _notificationService;

    public ChatController(
        IAuthenticationService authService,
        IChatFacadeService chatFacade,
        IConversationAccessControl accessControl,
        IUserContextProvider userContextProvider,
        IApiResponseBuilder responseBuilder,
        ILogger<ChatController> logger,
        IAdvancedSearchService advancedSearch,
        IAdvancedAnalyticsService advancedAnalytics,
        IAuditLoggingService auditLogging,
        IExportService exportService,
        IBatchOperationService batchOperation,
        INotificationService notificationService)
        : base(userContextProvider, responseBuilder, logger)
    {
        _authService = authService;
        _chatFacade = chatFacade;
        _accessControl = accessControl;
        _advancedSearch = advancedSearch;
        _advancedAnalytics = advancedAnalytics;
        _auditLogging = auditLogging;
        _exportService = exportService;
        _batchOperation = batchOperation;
        _notificationService = notificationService;
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

    [HttpPost("{conversationId}/send")]
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

    // Advanced Search Endpoints
    [HttpPost("search/conversations")]
    [Authorize]
    public async Task<IActionResult> SearchConversations([FromBody] SearchConversationsRequest request)
    {
        var userId = CurrentUserId;
        LogAction("SearchConversations", new { query = request.Query });

        var results = await _advancedSearch.SearchConversationsAsync(userId, request);
        await _auditLogging.LogActionAsync(userId, "SearchConversation", "Conversation", null);

        return Ok(results, "Search completed successfully");
    }

    [HttpGet("search/messages")]
    [Authorize]
    public async Task<IActionResult> SearchMessages([FromQuery] string query, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var userId = CurrentUserId;
        LogAction("SearchMessages", new { query });

        var results = await _advancedSearch.SearchMessagesAsync(userId, query, page, pageSize);
        await _auditLogging.LogActionAsync(userId, "SearchMessages", "Message", null);

        return Ok(results, "Message search completed successfully");
    }

    // Advanced Analytics Endpoints
    [HttpGet("analytics/advanced")]
    [Authorize]
    public async Task<IActionResult> GetAdvancedAnalytics([FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
    {
        var userId = CurrentUserId;
        LogAction("GetAdvancedAnalytics", new { fromDate, toDate });

        var analytics = await _advancedAnalytics.GetAdvancedAnalyticsAsync(userId, fromDate, toDate);

        return Ok(analytics, "Advanced analytics retrieved successfully");
    }

    [HttpGet("analytics/trends")]
    [Authorize]
    public async Task<IActionResult> GetMessageTrends([FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
    {
        var userId = CurrentUserId;
        LogAction("GetMessageTrends");

        var trends = await _advancedAnalytics.GetMessageTrendsByDayAsync(userId, fromDate, toDate);

        return Ok(trends, "Message trends retrieved successfully");
    }

    [HttpGet("analytics/intents")]
    [Authorize]
    public async Task<IActionResult> GetIntentDistribution()
    {
        var userId = CurrentUserId;
        LogAction("GetIntentDistribution");

        var distribution = await _advancedAnalytics.GetIntentDistributionAsync(userId);

        return Ok(distribution, "Intent distribution retrieved successfully");
    }

    // Batch Operations Endpoints
    [HttpPost("batch/delete")]
    [Authorize]
    public async Task<IActionResult> DeleteConversationsBatch([FromBody] List<int> conversationIds)
    {
        var userId = CurrentUserId;
        LogAction("DeleteConversationsBatch", new { count = conversationIds.Count });

        var result = await _batchOperation.DeleteConversationsBatchAsync(userId, conversationIds);
        await _auditLogging.LogActionAsync(userId, "BatchDelete", "Conversation", null, null,
            new Dictionary<string, object> { { "count", conversationIds.Count } });

        return Ok(result);
    }

    [HttpPost("batch/archive")]
    [Authorize]
    public async Task<IActionResult> ArchiveConversationsBatch([FromBody] List<int> conversationIds)
    {
        var userId = CurrentUserId;
        LogAction("ArchiveConversationsBatch", new { count = conversationIds.Count });

        var result = await _batchOperation.ArchiveConversationsBatchAsync(userId, conversationIds);
        await _auditLogging.LogActionAsync(userId, "BatchArchive", "Conversation", null, null,
            new Dictionary<string, object> { { "count", conversationIds.Count } });

        return Ok(result);
    }

    [HttpPost("batch/operation")]
    [Authorize]
    public async Task<IActionResult> ExecuteBatchOperation([FromBody] BatchOperationRequest request)
    {
        var userId = CurrentUserId;
        LogAction("ExecuteBatchOperation", new { operation = request.Operation });

        var result = await _batchOperation.ExecuteBatchOperationAsync(userId, request);

        return Ok(result);
    }

    // Export Endpoints
    [HttpPost("export/bulk")]
    [Authorize]
    public async Task<IActionResult> ExportConversationsBulk([FromBody] ExportRequest request)
    {
        var userId = CurrentUserId;
        LogAction("ExportConversationsBulk", new { format = request.Format, count = request.ConversationIds.Count });

        var bytes = await _exportService.ExportAsync(userId, request);
        var contentType = request.Format.ToLower() switch
        {
            "json" => "application/json",
            "csv" => "text/csv",
            "pdf" => "application/pdf",
            _ => "application/octet-stream"
        };

        await _auditLogging.LogActionAsync(userId, "Export", "Conversation", null, null,
            new Dictionary<string, object> { { "format", request.Format }, { "count", request.ConversationIds.Count } });

        return File(bytes, contentType, $"conversations_export_{DateTime.UtcNow:yyyyMMdd_HHmmss}.{request.Format.ToLower()}");
    }

    // Audit Log Endpoints
    [HttpGet("audit-logs")]
    [Authorize]
    public async Task<IActionResult> GetAuditLogs([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var userId = CurrentUserId;
        LogAction("GetAuditLogs");

        var logs = await _auditLogging.GetAuditLogsAsync(userId, page, pageSize);

        return Ok(logs, "Audit logs retrieved successfully");
    }

    [HttpGet("audit-logs/resource/{resourceType}/{resourceId}")]
    [Authorize]
    public async Task<IActionResult> GetResourceAuditLogs(string resourceType, int resourceId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var userId = CurrentUserId;
        LogAction("GetResourceAuditLogs", new { resourceType, resourceId });

        var logs = await _auditLogging.GetResourceAuditLogsAsync(resourceType, resourceId, page, pageSize);

        return Ok(logs, "Resource audit logs retrieved successfully");
    }

    // Notification Endpoints
    [HttpGet("notifications/unread")]
    [Authorize]
    public async Task<IActionResult> GetUnreadNotifications()
    {
        var userId = CurrentUserId;
        LogAction("GetUnreadNotifications");

        var notifications = await _notificationService.GetUnreadNotificationsAsync(userId);

        return Ok(notifications, "Unread notifications retrieved successfully");
    }

    [HttpPut("notifications/{notificationId}/read")]
    [Authorize]
    public async Task<IActionResult> MarkNotificationAsRead(int notificationId)
    {
        LogAction("MarkNotificationAsRead", new { notificationId });

        await _notificationService.MarkNotificationAsReadAsync(notificationId);

        return Ok("Notification marked as read");
    }

    [HttpPut("notifications/mark-all-read")]
    [Authorize]
    public async Task<IActionResult> MarkAllNotificationsAsRead()
    {
        var userId = CurrentUserId;
        LogAction("MarkAllNotificationsAsRead");

        await _notificationService.MarkAllNotificationsAsReadAsync(userId);
        await _auditLogging.LogActionAsync(userId, "MarkAllNotificationsAsRead", "Notification", null);

        return Ok("All notifications marked as read");
    }

    [HttpDelete("notifications/{notificationId}")]
    [Authorize]
    public async Task<IActionResult> DeleteNotification(int notificationId)
    {
        LogAction("DeleteNotification", new { notificationId });

        await _notificationService.DeleteNotificationAsync(notificationId);

        return Ok("Notification deleted successfully");
    }
}

