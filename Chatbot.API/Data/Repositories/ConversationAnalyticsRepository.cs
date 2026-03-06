using Chatbot.API.Data.Repositories.Interfaces;
using Chatbot.Core.Models.Entities;

namespace Chatbot.API.Data.Repositories;

public class ConversationAnalyticsRepository : Repository<ConversationAnalyticsEntity>, IConversationAnalyticsRepository
{
    public ConversationAnalyticsRepository(ChatbotDbContext context) : base(context) { }

    public async Task<ConversationAnalyticsEntity?> GetByConversationIdAsync(int conversationId) =>
        _context.Set<ConversationAnalyticsEntity>().FirstOrDefault(x => x.ConversationId == conversationId);

    public async Task<List<ConversationAnalyticsEntity>> GetUserAnalyticsByDateRangeAsync(int userId, DateTime startDate, DateTime endDate) =>
        _context.Set<ConversationAnalyticsEntity>()
              .Where(x => x.UserId == userId && x.UpdatedAt >= startDate && x.UpdatedAt <= endDate)
              .OrderByDescending(x => x.UpdatedAt)
              .ToList();

    public async Task<double> GetAverageSentimentAsync(int userId, int days = 30)
    {
        var since = DateTime.UtcNow.AddDays(-days);
        var analytics = _context.Set<ConversationAnalyticsEntity>().Where(x => x.UserId == userId && x.CreatedAt >= since).ToList();
        return analytics.Any() ? analytics.Average(x => x.AverageSentimentScore) : 0.0;
    }

    public async Task<List<ConversationAnalyticsEntity>> GetByEngagementScoreAsync(int userId, int minScore) =>
        _context.Set<ConversationAnalyticsEntity>()
              .Where(x => x.UserId == userId && x.EngagementScore >= minScore)
              .OrderByDescending(x => x.EngagementScore)
              .ToList();
}
