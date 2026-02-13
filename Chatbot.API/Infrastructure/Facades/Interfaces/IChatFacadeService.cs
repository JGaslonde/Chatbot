using Chatbot.API.Services.Analytics.Interfaces;
using Chatbot.API.Services.Core.Interfaces;
using Chatbot.API.Services.Export.Interfaces;
using Chatbot.Core.Models;
using Chatbot.Core.Models.Responses;
using Chatbot.Core.Models.Entities;

namespace Chatbot.API.Infrastructure.Facades.Interfaces;

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
