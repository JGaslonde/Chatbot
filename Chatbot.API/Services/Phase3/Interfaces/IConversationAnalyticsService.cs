using Chatbot.Core.Models.Entities;
using Chatbot.Core.Models.Requests;
using Chatbot.Core.Models.Responses;

namespace Chatbot.API.Services.Phase3.Interfaces;

public interface IConversationAnalyticsService
{
    Task<ConversationAnalyticsDto?> GetAnalyticsAsync(int conversationId);
    Task<List<ConversationAnalyticsDto>> GetUserAnalyticsByDateRangeAsync(int userId, DateTime startDate, DateTime endDate);
    Task<double> GetAverageSentimentAsync(int userId, int days = 30);
    Task<AnalyticsSummaryDto> GetAnalyticsSummaryAsync(int userId, int days = 30);
    Task<ConversationAnalyticsEntity> CreateOrUpdateAnalyticsAsync(int conversationId, int userId, AnalyticsRequest request);
}
