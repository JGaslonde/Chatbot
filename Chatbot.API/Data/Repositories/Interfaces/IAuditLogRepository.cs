using Chatbot.Core.Models.Entities;

namespace Chatbot.API.Data.Repositories.Interfaces;

public interface IAuditLogRepository : IRepository<AuditLog>
{
    Task<IEnumerable<AuditLog>> GetUserAuditLogsAsync(int userId);
    Task<IEnumerable<AuditLog>> GetResourceAuditLogsAsync(string resourceType, int resourceId);
}
