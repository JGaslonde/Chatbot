using Chatbot.API.Data.Repositories.Interfaces;
using Chatbot.Core.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Chatbot.API.Data.Repositories;

public class ScheduledReportRepository : Repository<ScheduledReport>, IScheduledReportRepository
{
    public ScheduledReportRepository(ChatbotDbContext context) : base(context) { }

    public async Task<IEnumerable<ScheduledReport>> GetUserReportsAsync(int userId)
    {
        return await _context.ScheduledReports
            .Where(sr => sr.UserId == userId)
            .OrderByDescending(sr => sr.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<ScheduledReport>> GetDueReportsAsync()
    {
        var now = DateTime.UtcNow;
        return await _context.ScheduledReports
            .Where(sr => sr.IsActive && sr.NextGeneratedAt <= now)
            .ToListAsync();
    }
}
