using System.Text.Json;
using Chatbot.Core.Models.Entities;
using Chatbot.Core.Models.Responses;
using Chatbot.API.Data.Repositories.Interfaces;
using Chatbot.API.Services.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Chatbot.API.Services.Core;

public class AuditLoggingService : IAuditLoggingService
{
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly ILogger<AuditLoggingService> _logger;

    public AuditLoggingService(
        IAuditLogRepository auditLogRepository,
        ILogger<AuditLoggingService> logger)
    {
        _auditLogRepository = auditLogRepository;
        _logger = logger;
    }

    public async Task LogActionAsync(
        int userId,
        string action,
        string resourceType,
        int? resourceId = null,
        string? ipAddress = null,
        Dictionary<string, object>? details = null)
    {
        try
        {
            var auditLog = new AuditLog
            {
                UserId = userId,
                Action = action,
                ResourceType = resourceType,
                ResourceId = resourceId,
                Timestamp = DateTime.UtcNow,
                IpAddress = ipAddress,
                Details = details != null ? JsonSerializer.Serialize(details) : null
            };

            await _auditLogRepository.AddAsync(auditLog);
            _logger.LogDebug("Audit log created for user {UserId}: {Action}", userId, action);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error logging audit for user {UserId}", userId);
            // Don't throw - audit logging failures shouldn't break the app
        }
    }

    public async Task<AuditLogResponse> GetAuditLogsAsync(int userId, int page = 1, int pageSize = 20)
    {
        try
        {
            var logs = (await _auditLogRepository.GetUserAuditLogsAsync(userId)).ToList();
            var totalCount = logs.Count;
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            var skip = (page - 1) * pageSize;
            var pagedLogs = logs.Skip(skip).Take(pageSize).ToList();

            var entries = pagedLogs.Select(log => new AuditLogEntry(
                log.Id,
                log.UserId,
                log.Action,
                log.ResourceType,
                log.ResourceId,
                log.Timestamp,
                log.IpAddress ?? "Unknown",
                log.Details != null ? JsonSerializer.Deserialize<Dictionary<string, object>>(log.Details) : null
            )).ToList();

            return new AuditLogResponse(entries, totalCount, page, pageSize);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving audit logs for user {UserId}", userId);
            throw;
        }
    }

    public async Task<AuditLogResponse> GetResourceAuditLogsAsync(
        string resourceType,
        int resourceId,
        int page = 1,
        int pageSize = 20)
    {
        try
        {
            var logs = (await _auditLogRepository.GetResourceAuditLogsAsync(resourceType, resourceId)).ToList();
            var totalCount = logs.Count;
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            var skip = (page - 1) * pageSize;
            var pagedLogs = logs.Skip(skip).Take(pageSize).ToList();

            var entries = pagedLogs.Select(log => new AuditLogEntry(
                log.Id,
                log.UserId,
                log.Action,
                log.ResourceType,
                log.ResourceId,
                log.Timestamp,
                log.IpAddress ?? "Unknown",
                log.Details != null ? JsonSerializer.Deserialize<Dictionary<string, object>>(log.Details) : null
            )).ToList();

            return new AuditLogResponse(entries, totalCount, page, pageSize);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving audit logs for resource {ResourceType}:{ResourceId}", resourceType, resourceId);
            throw;
        }
    }
}
