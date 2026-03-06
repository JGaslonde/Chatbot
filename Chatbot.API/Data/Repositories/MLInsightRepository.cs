using Chatbot.API.Data.Repositories.Interfaces;
using Chatbot.Core.Models.Entities;

namespace Chatbot.API.Data.Repositories;

public class MLInsightRepository : Repository<MLInsight>, IMLInsightRepository
{
    public MLInsightRepository(ChatbotDbContext context) : base(context) { }

    public async Task<List<MLInsight>> GetUserInsightsAsync(int userId) =>
        _context.Set<MLInsight>().Where(x => x.UserId == userId)
              .OrderByDescending(x => x.GeneratedAt)
              .ToList();

    public async Task<List<MLInsight>> GetByInsightTypeAsync(int userId, string insightType) =>
        _context.Set<MLInsight>().Where(x => x.UserId == userId && x.InsightType == insightType)
              .OrderByDescending(x => x.Confidence)
              .ToList();

    public async Task<List<MLInsight>> GetUnreviewedInsightsAsync(int userId) =>
        _context.Set<MLInsight>().Where(x => x.UserId == userId && !x.IsReviewed)
              .OrderByDescending(x => x.GeneratedAt)
              .ToList();

    public async Task<List<MLInsight>> GetHighConfidenceInsightsAsync(int userId, double minConfidence) =>
        _context.Set<MLInsight>().Where(x => x.UserId == userId && x.Confidence >= minConfidence)
              .OrderByDescending(x => x.Confidence)
              .ToList();
}
