using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using Chatbot.API.Services.Core.Interfaces;
using Chatbot.Core.Models.Entities;

namespace Chatbot.API.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly IConversationService _conversationService;
    private readonly ILogger<ChatHub> _logger;

    public ChatHub(IConversationService conversationService, ILogger<ChatHub> logger)
    {
        _conversationService = conversationService;
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        var username = Context.User?.Identity?.Name ?? "Anonymous";
        _logger.LogInformation("User {Username} connected with connection ID {ConnectionId}", username, Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var username = Context.User?.Identity?.Name ?? "Anonymous";
        _logger.LogInformation("User {Username} disconnected", username);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task JoinConversation(int conversationId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"conversation_{conversationId}");
        _logger.LogInformation("Connection {ConnectionId} joined conversation {ConversationId}", Context.ConnectionId, conversationId);

        // Notify others in the conversation
        await Clients.OthersInGroup($"conversation_{conversationId}")
            .SendAsync("UserJoined", Context.User?.Identity?.Name ?? "Anonymous");
    }

    public async Task LeaveConversation(int conversationId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"conversation_{conversationId}");
        _logger.LogInformation("Connection {ConnectionId} left conversation {ConversationId}", Context.ConnectionId, conversationId);

        // Notify others in the conversation
        await Clients.OthersInGroup($"conversation_{conversationId}")
            .SendAsync("UserLeft", Context.User?.Identity?.Name ?? "Anonymous");
    }

    public async Task SendMessage(int conversationId, string message)
    {
        try
        {
            // Add user message
            var userMessage = await _conversationService.AddMessageAsync(conversationId, message, MessageSender.User);

            // Send user message to all clients in the conversation
            await Clients.Group($"conversation_{conversationId}")
                .SendAsync("ReceiveMessage", new
                {
                    userMessage.Id,
                    userMessage.Content,
                    Sender = userMessage.Sender.ToString(),
                    userMessage.SentAt,
                    Sentiment = userMessage.Sentiment.ToString(),
                    userMessage.SentimentScore,
                    userMessage.DetectedIntent,
                    userMessage.IntentConfidence
                });

            // Generate and send bot response
            var botResponseText = await _conversationService.GenerateBotResponseAsync(conversationId, message);
            var botMessage = await _conversationService.AddMessageAsync(conversationId, botResponseText, MessageSender.Bot);

            // Send bot message to all clients in the conversation
            await Clients.Group($"conversation_{conversationId}")
                .SendAsync("ReceiveMessage", new
                {
                    botMessage.Id,
                    botMessage.Content,
                    Sender = botMessage.Sender.ToString(),
                    botMessage.SentAt,
                    Sentiment = botMessage.Sentiment.ToString(),
                    botMessage.SentimentScore,
                    botMessage.DetectedIntent,
                    botMessage.IntentConfidence
                });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending message in conversation {ConversationId}", conversationId);
            await Clients.Caller.SendAsync("Error", "Failed to send message");
        }
    }

    public async Task SendTypingIndicator(int conversationId, bool isTyping)
    {
        // Notify others in the conversation about typing status
        await Clients.OthersInGroup($"conversation_{conversationId}")
            .SendAsync("UserTyping", Context.User?.Identity?.Name ?? "Anonymous", isTyping);
    }

    // Phase 2: Webhook real-time updates
    public async Task SubscribeToWebhookUpdates(int webhookId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"webhook_{webhookId}");
        _logger.LogInformation("Connection {ConnectionId} subscribed to webhook {WebhookId} updates", Context.ConnectionId, webhookId);
    }

    public async Task UnsubscribeFromWebhookUpdates(int webhookId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"webhook_{webhookId}");
        _logger.LogInformation("Connection {ConnectionId} unsubscribed from webhook {WebhookId} updates", Context.ConnectionId, webhookId);
    }

    // Phase 2: Import progress real-time updates
    public async Task SubscribeToImportProgress(int importJobId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"import_{importJobId}");
        _logger.LogInformation("Connection {ConnectionId} subscribed to import {ImportJobId} progress", Context.ConnectionId, importJobId);
    }

    public async Task UnsubscribeFromImportProgress(int importJobId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"import_{importJobId}");
        _logger.LogInformation("Connection {ConnectionId} unsubscribed from import {ImportJobId} progress", Context.ConnectionId, importJobId);
    }

    // Phase 2: Report generation status updates
    public async Task SubscribeToReportGeneration(int reportId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"report_{reportId}");
        _logger.LogInformation("Connection {ConnectionId} subscribed to report {ReportId} generation", Context.ConnectionId, reportId);
    }

    public async Task UnsubscribeFromReportGeneration(int reportId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"report_{reportId}");
        _logger.LogInformation("Connection {ConnectionId} unsubscribed from report {ReportId} generation", Context.ConnectionId, reportId);
    }

    // Internal method to broadcast webhook delivery status (called by WebhookService)
    public async Task BroadcastWebhookDelivered(int webhookId, string eventType, bool success, string? errorMessage = null)
    {
        await Clients.Group($"webhook_{webhookId}")
            .SendAsync("WebhookDelivered", new
            {
                WebhookId = webhookId,
                EventType = eventType,
                Success = success,
                ErrorMessage = errorMessage,
                Timestamp = DateTime.UtcNow
            });

        _logger.LogInformation("Webhook {WebhookId} delivery broadcasted: Success={Success}", webhookId, success);
    }

    // Internal method to broadcast import progress (called by ImportService)
    public async Task BroadcastImportProgress(int importJobId, int processed, int total, int failed)
    {
        var progressPercentage = total > 0 ? (processed * 100 / total) : 0;

        await Clients.Group($"import_{importJobId}")
            .SendAsync("ImportProgress", new
            {
                ImportJobId = importJobId,
                ProcessedRecords = processed,
                TotalRecords = total,
                FailedRecords = failed,
                ProgressPercentage = progressPercentage,
                Timestamp = DateTime.UtcNow
            });

        _logger.LogInformation("Import {ImportJobId} progress broadcasted: {Processed}/{Total} ({Percentage}%)",
            importJobId, processed, total, progressPercentage);
    }

    // Internal method to broadcast report generation status (called by ReportingService)
    public async Task BroadcastReportGenerationStatus(int reportId, string status, string? downloadUrl = null, string? errorMessage = null)
    {
        await Clients.Group($"report_{reportId}")
            .SendAsync("ReportGenerationStatus", new
            {
                ReportId = reportId,
                Status = status, // "generating", "completed", "failed"
                DownloadUrl = downloadUrl,
                ErrorMessage = errorMessage,
                Timestamp = DateTime.UtcNow
            });

        _logger.LogInformation("Report {ReportId} status broadcasted: {Status}", reportId, status);
    }

    // ========== ENHANCED REAL-TIME FEATURES ==========

    /// <summary>
    /// Subscribe to system notifications (admin notifications, alerts, etc.)
    /// </summary>
    public async Task SubscribeToSystemNotifications()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "system_notifications");
        _logger.LogInformation("Connection {ConnectionId} subscribed to system notifications", Context.ConnectionId);
    }

    /// <summary>
    /// Subscribe to analytics real-time updates
    /// </summary>
    public async Task SubscribeToAnalyticsUpdates(int? conversationId = null)
    {
        var groupName = conversationId.HasValue ? $"analytics_conversation_{conversationId}" : "analytics_global";
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        _logger.LogInformation("Connection {ConnectionId} subscribed to analytics updates: {GroupName}", Context.ConnectionId, groupName);
    }

    /// <summary>
    /// Subscribe to user activity feed
    /// </summary>
    public async Task SubscribeToActivityFeed()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "activity_feed");
        _logger.LogInformation("Connection {ConnectionId} subscribed to activity feed", Context.ConnectionId);
    }

    /// <summary>
    /// Broadcast a system notification to all connected users
    /// </summary>
    public async Task BroadcastSystemNotification(string title, string message, string notificationType = "info", string? actionUrl = null)
    {
        await Clients.Group("system_notifications")
            .SendAsync("SystemNotification", new
            {
                Title = title,
                Message = message,
                Type = notificationType, // "info", "warning", "error", "success"
                ActionUrl = actionUrl,
                Timestamp = DateTime.UtcNow
            });

        _logger.LogInformation("System notification broadcasted: {Title}", title);
    }

    /// <summary>
    /// Broadcast real-time analytics update
    /// </summary>
    public async Task BroadcastAnalyticsUpdate(int? conversationId, string metricName, double value, Dictionary<string, object>? metadata = null)
    {
        var groupName = conversationId.HasValue ? $"analytics_conversation_{conversationId}" : "analytics_global";

        await Clients.Group(groupName)
            .SendAsync("AnalyticsUpdate", new
            {
                ConversationId = conversationId,
                MetricName = metricName,
                Value = value,
                Metadata = metadata,
                Timestamp = DateTime.UtcNow
            });

        _logger.LogInformation("Analytics update broadcasted: {MetricName}={Value}", metricName, value);
    }

    /// <summary>
    /// Broadcast user activity to activity feed
    /// </summary>
    public async Task BroadcastUserActivity(string userId, string activityType, string description, string? resourceId = null)
    {
        await Clients.Group("activity_feed")
            .SendAsync("UserActivity", new
            {
                UserId = userId,
                ActivityType = activityType, // "conversation_started", "message_sent", "search_executed", etc.
                Description = description,
                ResourceId = resourceId,
                Timestamp = DateTime.UtcNow
            });

        _logger.LogInformation("User activity broadcasted: {UserId} - {ActivityType}", userId, activityType);
    }

    /// <summary>
    /// Broadcast conversation status update
    /// </summary>
    public async Task BroadcastConversationUpdate(int conversationId, string updateType, Dictionary<string, object>? details = null)
    {
        await Clients.Group($"conversation_{conversationId}")
            .SendAsync("ConversationUpdate", new
            {
                ConversationId = conversationId,
                UpdateType = updateType, // "status_changed", "sentiment_updated", "metadata_changed"
                Details = details,
                Timestamp = DateTime.UtcNow
            });

        _logger.LogInformation("Conversation {ConversationId} update broadcasted: {UpdateType}", conversationId, updateType);
    }

    /// <summary>
    /// Broadcast sentiment analysis update for a message
    /// </summary>
    public async Task BroadcastSentimentAnalysis(int conversationId, int messageId, string sentiment, double score)
    {
        await Clients.Group($"conversation_{conversationId}")
            .SendAsync("SentimentAnalysis", new
            {
                MessageId = messageId,
                Sentiment = sentiment,
                Score = score,
                Timestamp = DateTime.UtcNow
            });

        _logger.LogInformation("Sentiment analysis update broadcasted: MessageId={MessageId}, Sentiment={Sentiment}, Score={Score}",
            messageId, sentiment, score);
    }

    /// <summary>
    /// Broadcast intent recognition result
    /// </summary>
    public async Task BroadcastIntentRecognition(int conversationId, int messageId, string intent, double confidence)
    {
        await Clients.Group($"conversation_{conversationId}")
            .SendAsync("IntentRecognition", new
            {
                MessageId = messageId,
                Intent = intent,
                Confidence = confidence,
                Timestamp = DateTime.UtcNow
            });

        _logger.LogInformation("Intent recognition update broadcasted: MessageId={MessageId}, Intent={Intent}, Confidence={Confidence}",
            messageId, intent, confidence);
    }

    /// <summary>
    /// Broadcast real-time statistics update
    /// </summary>
    public async Task BroadcastStatisticsUpdate(Dictionary<string, object> statistics)
    {
        await Clients.All
            .SendAsync("StatisticsUpdate", new
            {
                Statistics = statistics,
                Timestamp = DateTime.UtcNow
            });

        _logger.LogInformation("Statistics update broadcasted");
    }
}
