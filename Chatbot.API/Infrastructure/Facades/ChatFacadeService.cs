using Chatbot.API.Services.Core;
using Chatbot.API.Services.Analytics;
using Chatbot.API.Services.Export;
using Chatbot.Core.Models;
using Chatbot.Core.Models.Entities;

namespace Chatbot.API.Infrastructure.Facades;

/// <summary>
/// Facade service that coordinates multiple specialized services.
/// Implements Facade Pattern - Reduces controller dependencies and provides simple interface.
/// Applies Single Responsibility - Controllers only depend on this facade, not multiple services.
/// Dependency Inversion - Controllers depend on abstraction, not concrete services.
/// </summary>
public interface IChatFacadeService
{
    // Conversation operations
    Task<Conversation> CreateConversationAsync(int userId, string? title = null);
    Task<Conversation?> GetConversationAsync(int conversationId);
    Task<ChatMessageResponse> SendMessageAsync(int conversationId, string userMessage);
    Task<MessageHistoryResponse> GetConversationHistoryAsync(int conversationId);

    // Data export operations
    Task<byte[]> ExportConversationJsonAsync(int conversationId);
    Task<byte[]> ExportConversationCsvAsync(int conversationId);

    // Analytics operations
    Task<ConversationAnalytics> GetAnalyticsAsync(int userId, DateTime? startDate = null, DateTime? endDate = null);
    Task<List<SentimentTrend>> GetSentimentTrendsAsync(int userId, int days = 7);
    Task<List<IntentDistribution>> GetIntentDistributionAsync(int userId, int days = 30);

    // User preferences operations
    Task<UserPreferences> GetPreferencesAsync(int userId);
    Task<UserPreferences> UpdatePreferencesAsync(int userId, UserPreferences preferences);
}

public class ChatFacadeService : IChatFacadeService
{
    private readonly IConversationService _conversationService;
    private readonly IConversationExportService _exportService;
    private readonly IConversationAnalyticsService _analyticsService;
    private readonly IUserPreferencesService _preferencesService;
    private readonly ILogger<ChatFacadeService> _logger;

    public ChatFacadeService(
        IConversationService conversationService,
        IConversationExportService exportService,
        IConversationAnalyticsService analyticsService,
        IUserPreferencesService preferencesService,
        ILogger<ChatFacadeService> logger)
    {
        _conversationService = conversationService;
        _exportService = exportService;
        _analyticsService = analyticsService;
        _preferencesService = preferencesService;
        _logger = logger;
    }

    public async Task<Conversation> CreateConversationAsync(int userId, string? title = null)
    {
        _logger.LogInformation("Creating conversation for user {UserId}", userId);
        return await _conversationService.CreateConversationAsync(userId, title);
    }

    public async Task<Conversation?> GetConversationAsync(int conversationId)
    {
        _logger.LogInformation("Retrieving conversation {ConversationId}", conversationId);
        return await _conversationService.GetConversationAsync(conversationId);
    }

    public async Task<ChatMessageResponse> SendMessageAsync(int conversationId, string userMessage)
    {
        _logger.LogInformation("Sending message to conversation {ConversationId}", conversationId);

        var userMsg = await _conversationService.AddMessageAsync(conversationId, userMessage, MessageSender.User);
        var botResponse = await _conversationService.GenerateBotResponseAsync(conversationId, userMessage);
        var botMsg = await _conversationService.AddMessageAsync(conversationId, botResponse, MessageSender.Bot);

        if (conversationId % 5 == 0)
        {
            await _conversationService.UpdateConversationSummaryAsync(conversationId);
        }

        return new ChatMessageResponse(
            botMsg.Content,
            botMsg.SentAt,
            botMsg.DetectedIntent ?? "unknown",
            botMsg.IntentConfidence,
            botMsg.Sentiment.ToString(),
            botMsg.SentimentScore,
            conversationId);
    }

    public async Task<MessageHistoryResponse> GetConversationHistoryAsync(int conversationId)
    {
        _logger.LogInformation("Retrieving history for conversation {ConversationId}", conversationId);
        var conversation = await _conversationService.GetConversationAsync(conversationId);

        if (conversation == null)
            throw new InvalidOperationException($"Conversation {conversationId} not found");

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

        return new MessageHistoryResponse(conversationId, messageDtos);
    }

    public async Task<byte[]> ExportConversationJsonAsync(int conversationId)
    {
        _logger.LogInformation("Exporting conversation {ConversationId} as JSON", conversationId);
        return await _exportService.ExportToJsonBytesAsync(conversationId);
    }

    public async Task<byte[]> ExportConversationCsvAsync(int conversationId)
    {
        _logger.LogInformation("Exporting conversation {ConversationId} as CSV", conversationId);
        return await _exportService.ExportToCsvBytesAsync(conversationId);
    }

    public async Task<ConversationAnalytics> GetAnalyticsAsync(int userId, DateTime? startDate = null, DateTime? endDate = null)
    {
        _logger.LogInformation("Retrieving analytics for user {UserId}", userId);
        return await _analyticsService.GetAnalyticsAsync(userId, startDate, endDate);
    }

    public async Task<List<SentimentTrend>> GetSentimentTrendsAsync(int userId, int days = 7)
    {
        _logger.LogInformation("Retrieving sentiment trends for user {UserId}", userId);
        return await _analyticsService.GetSentimentTrendsAsync(userId, days);
    }

    public async Task<List<IntentDistribution>> GetIntentDistributionAsync(int userId, int days = 30)
    {
        _logger.LogInformation("Retrieving intent distribution for user {UserId}", userId);
        return await _analyticsService.GetIntentDistributionAsync(userId, days);
    }

    public async Task<UserPreferences> GetPreferencesAsync(int userId)
    {
        _logger.LogInformation("Retrieving preferences for user {UserId}", userId);
        return await _preferencesService.GetPreferencesAsync(userId);
    }

    public async Task<UserPreferences> UpdatePreferencesAsync(int userId, UserPreferences preferences)
    {
        _logger.LogInformation("Updating preferences for user {UserId}", userId);
        return await _preferencesService.UpdatePreferencesAsync(userId, preferences);
    }
}
