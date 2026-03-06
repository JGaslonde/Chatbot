using Chatbot.Core.Models.Entities;

namespace Chatbot.API.Data.Repositories.Interfaces;

public interface IConversationAnalyticsRepository : IRepository<ConversationAnalyticsEntity>
{
    Task<ConversationAnalyticsEntity?> GetByConversationIdAsync(int conversationId);
    Task<List<ConversationAnalyticsEntity>> GetUserAnalyticsByDateRangeAsync(int userId, DateTime startDate, DateTime endDate);
    Task<double> GetAverageSentimentAsync(int userId, int days = 30);
    Task<List<ConversationAnalyticsEntity>> GetByEngagementScoreAsync(int userId, int minScore);
}
