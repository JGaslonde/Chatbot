using Chatbot.Core.Models.Entities;
using Chatbot.Core.Models.Requests;
using Chatbot.Core.Models.Responses;

namespace Chatbot.API.Services.Phase3.Interfaces;

public interface IMLInsightService
{
    Task<List<MLInsightDto>> GetUserInsightsAsync(int userId);
    Task<List<MLInsightDto>> GetByInsightTypeAsync(int userId, string insightType);
    Task<List<MLInsightDto>> GetUnreviewedInsightsAsync(int userId);
    Task<MLInsightDto> MarkAsReviewedAsync(int insightId);
    Task<MLInsight> GenerateInsightAsync(int userId, int conversationId, string insightType, double confidence, string value);
}
