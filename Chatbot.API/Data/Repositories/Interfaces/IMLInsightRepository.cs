using Chatbot.Core.Models.Entities;

namespace Chatbot.API.Data.Repositories.Interfaces;

public interface IMLInsightRepository : IRepository<MLInsight>
{
    Task<List<MLInsight>> GetUserInsightsAsync(int userId);
    Task<List<MLInsight>> GetByInsightTypeAsync(int userId, string insightType);
    Task<List<MLInsight>> GetUnreviewedInsightsAsync(int userId);
    Task<List<MLInsight>> GetHighConfidenceInsightsAsync(int userId, double minConfidence = 0.8);
}
