using Chatbot.API.Services.Analytics;
using Chatbot.API.Services.Core;
using Chatbot.API.Services.Export;
using Chatbot.Core.Models;
using Chatbot.Core.Models.Entities;
using Microsoft.Extensions.Logging;

namespace Chatbot.API.Infrastructure.Facades;

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

        // Add user message
        var userMsg = await _conversationService.AddMessageAsync(conversationId, userMessage, MessageSender.User);

        // Generate bot response
        var botResponse = await _conversationService.GenerateBotResponseAsync(conversationId, userMessage);
        var botMsg = await _conversationService.AddMessageAsync(conversationId, botResponse, MessageSender.Bot);

        return new ChatMessageResponse(
            Message: botResponse,
            Timestamp: botMsg.SentAt,
            Intent: botMsg.DetectedIntent ?? "Unknown",
            IntentConfidence: botMsg.IntentConfidence,
            Sentiment: botMsg.Sentiment.ToString(),
            SentimentScore: botMsg.SentimentScore,
            ConversationId: conversationId
        );
    }

    public async Task<MessageHistoryResponse> GetConversationHistoryAsync(int conversationId)
    {
        _logger.LogInformation("Retrieving conversation history for {ConversationId}", conversationId);
        var messages = await _conversationService.GetConversationHistoryAsync(conversationId);
        
        var messageDtos = messages.Select(m => new MessageDto(
            Id: m.Id,
            Content: m.Content,
            Sender: m.Sender.ToString(),
            SentAt: m.SentAt,
            Sentiment: m.Sentiment.ToString(),
            Intent: m.DetectedIntent,
            SentimentScore: m.SentimentScore
        )).ToList();
        
        return new MessageHistoryResponse(
            ConversationId: conversationId,
            Messages: messageDtos
        );
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
        _logger.LogInformation("Getting analytics for user {UserId}", userId);
        return await _analyticsService.GetAnalyticsAsync(userId, startDate, endDate);
    }

    public async Task<List<SentimentTrend>> GetSentimentTrendsAsync(int userId, int days = 7)
    {
        _logger.LogInformation("Getting sentiment trends for user {UserId}", userId);
        return await _analyticsService.GetSentimentTrendsAsync(userId, days);
    }

    public async Task<List<IntentDistribution>> GetIntentDistributionAsync(int userId, int days = 30)
    {
        _logger.LogInformation("Getting intent distribution for user {UserId}", userId);
        return await _analyticsService.GetIntentDistributionAsync(userId, days);
    }

    public async Task<UserPreferences> GetPreferencesAsync(int userId)
    {
        _logger.LogInformation("Getting preferences for user {UserId}", userId);
        return await _preferencesService.GetPreferencesAsync(userId);
    }

    public async Task<UserPreferences> UpdatePreferencesAsync(int userId, UserPreferences preferences)
    {
        _logger.LogInformation("Updating preferences for user {UserId}", userId);
        return await _preferencesService.UpdatePreferencesAsync(userId, preferences);
    }
}
