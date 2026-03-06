using Chatbot.Core.Models.Responses;

namespace Chatbot.API.Services.Admin.Interfaces;

public interface IAuditService
{
    Task<AuditLogResponse> GetAuditLogsAsync(
        int page,
        int pageSize,
        string? userId = null,
        string? action = null,
        DateTime? startDate = null,
        DateTime? endDate = null);

    Task<AuditLogEntry?> GetAuditLogEntryAsync(int id);
    Task LogActionAsync(int userId, string action, string resourceType, int? resourceId, Dictionary<string, object>? details = null);
    Task<int> ClearOldAuditLogsAsync(int daysToKeep);
}
