using Chatbot.Core.Models.Responses;

namespace Chatbot.API.Services.Phase3.Interfaces;

public interface IUserSegmentationService
{
    Task<UserSegmentDto?> GetUserSegmentAsync(int userId);
    Task<List<UserSegmentDto>> GetSegmentByBehaviorAsync(string behavioralSegment);
    Task<List<UserSegmentDto>> GetChurnRiskUsersAsync(double minRiskScore = 0.7);
    Task<UserSegmentDto> AnalyzeUserSegmentAsync(int userId);
    Task<ChurnPredictionDto> PredictChurnAsync(int userId);
}
