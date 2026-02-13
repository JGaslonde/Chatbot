using Chatbot.API.Data.Context;
using Chatbot.Core.Models.Entities;
using Chatbot.API.Data.Repositories.Interfaces;

namespace Chatbot.API.Data.Repositories;

public class AuditLogRepository : Repository<AuditLog>, IAuditLogRepository
{
    public AuditLogRepository(ChatbotDbContext context) : base(context) { }

    public async Task<IEnumerable<AuditLog>> GetUserAuditLogsAsync(int userId)
    {
        return await Task.FromResult(
            _context.AuditLogs
                .Where(al => al.UserId == userId)
                .OrderByDescending(al => al.Timestamp)
                .ToList()
        );
    }

    public async Task<IEnumerable<AuditLog>> GetResourceAuditLogsAsync(string resourceType, int resourceId)
    {
        return await Task.FromResult(
            _context.AuditLogs
                .Where(al => al.ResourceType == resourceType && al.ResourceId == resourceId)
                .OrderByDescending(al => al.Timestamp)
                .ToList()
        );
    }
}
