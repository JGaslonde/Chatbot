using Chatbot.Core.Models.Entities;

namespace Chatbot.API.Data.Repositories.Interfaces;

public interface IUserSegmentRepository : IRepository<UserSegment>
{
    Task<UserSegment?> GetByUserIdAsync(int userId);
    Task<List<UserSegment>> GetByBehavioralSegmentAsync(string segment);
    Task<List<UserSegment>> GetChurnRiskUsersAsync(double minRiskScore = 0.7);
    Task<List<UserSegment>> GetEngagementLevelAsync(string engagementLevel);
}
