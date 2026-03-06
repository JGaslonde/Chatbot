using Chatbot.API.Services.Admin.Interfaces;
using Chatbot.Core.Models.Responses;

namespace Chatbot.API.Services.Admin;

public class AuditService : IAuditService
{
    private readonly ILogger<AuditService> _logger;
    private readonly List<AuditLogEntry> _auditLogs; // In-memory for demo; use DB in production

    public AuditService(ILogger<AuditService> logger)
    {
        _logger = logger;
        _auditLogs = new List<AuditLogEntry>();
    }

    public async Task<AuditLogResponse> GetAuditLogsAsync(
        int page,
        int pageSize,
        string? userId = null,
        string? action = null,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        try
        {
            var query = _auditLogs.AsEnumerable();

            if (!string.IsNullOrEmpty(userId))
                query = query.Where(x => x.UserId.ToString() == userId);

            if (!string.IsNullOrEmpty(action))
                query = query.Where(x => x.Action.Contains(action, StringComparison.OrdinalIgnoreCase));

            if (startDate.HasValue)
                query = query.Where(x => x.Timestamp >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(x => x.Timestamp <= endDate.Value);

            var totalCount = query.Count();
            var logs = query
                .OrderByDescending(x => x.Timestamp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return await Task.FromResult(new AuditLogResponse(
                Logs: logs,
                TotalCount: totalCount,
                Page: page,
                PageSize: pageSize
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting audit logs: {ex.Message}");
            throw;
        }
    }

    public async Task<AuditLogEntry?> GetAuditLogEntryAsync(int id)
    {
        try
        {
            return await Task.FromResult(_auditLogs.FirstOrDefault(x => x.Id == id));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting audit log entry: {ex.Message}");
            throw;
        }
    }

    public async Task LogActionAsync(
        int userId,
        string action,
        string resourceType,
        int? resourceId,
        Dictionary<string, object>? details = null)
    {
        try
        {
            var entry = new AuditLogEntry(
                Id: _auditLogs.Count + 1,
                UserId: userId,
                Action: action,
                ResourceType: resourceType,
                ResourceId: resourceId,
                Timestamp: DateTime.UtcNow,
                IpAddress: "127.0.0.1", // Should be from request context
                Details: details
            );

            _auditLogs.Add(entry);
            _logger.LogInformation($"Logged action: {action} by user {userId}");
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error logging action: {ex.Message}");
            throw;
        }
    }

    public async Task<int> ClearOldAuditLogsAsync(int daysToKeep)
    {
        try
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-daysToKeep);
            var beforeCount = _auditLogs.Count;
            _auditLogs.RemoveAll(x => x.Timestamp < cutoffDate);
            var deletedCount = beforeCount - _auditLogs.Count;

            _logger.LogInformation($"Cleared {deletedCount} old audit logs (keeping last {daysToKeep} days)");
            return await Task.FromResult(deletedCount);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error clearing old audit logs: {ex.Message}");
            throw;
        }
    }
}
