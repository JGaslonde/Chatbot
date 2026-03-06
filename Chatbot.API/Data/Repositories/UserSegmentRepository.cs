using Chatbot.API.Data.Repositories.Interfaces;
using Chatbot.Core.Models.Entities;

namespace Chatbot.API.Data.Repositories;

public class UserSegmentRepository : Repository<UserSegment>, IUserSegmentRepository
{
    public UserSegmentRepository(ChatbotDbContext context) : base(context) { }

    public async Task<UserSegment?> GetByUserIdAsync(int userId) =>
        _context.Set<UserSegment>().FirstOrDefault(x => x.UserId == userId);

    public async Task<List<UserSegment>> GetByBehavioralSegmentAsync(string segment) =>
        _context.Set<UserSegment>().Where(x => x.BehavioralSegment == segment)
              .OrderByDescending(x => x.ChurnRiskScore)
              .ToList();

    public async Task<List<UserSegment>> GetChurnRiskUsersAsync(double minRiskScore) =>
        _context.Set<UserSegment>().Where(x => x.ChurnRiskScore >= minRiskScore)
              .OrderByDescending(x => x.ChurnRiskScore)
              .ToList();

    public async Task<List<UserSegment>> GetEngagementLevelAsync(string engagementLevel) =>
        _context.Set<UserSegment>().Where(x => x.EngagementLevel == engagementLevel)
              .ToList();
}
