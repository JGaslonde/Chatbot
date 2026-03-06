using Chatbot.Core.Models.Requests;
using Chatbot.Core.Models.Responses;

namespace Chatbot.API.Services.Core.Interfaces;

/// <summary>
/// Service for audit logging of user actions.
/// </summary>
public interface IAuditLoggingService
{
    /// <summary>
    /// Logs a user action.
    /// </summary>
    Task LogActionAsync(
        int userId,
        string action,
        string resourceType,
        int? resourceId = null,
        string? ipAddress = null,
        Dictionary<string, object>? details = null);

    /// <summary>
    /// Gets audit logs for a user with pagination.
    /// </summary>
    Task<AuditLogResponse> GetAuditLogsAsync(
        int userId,
        int page = 1,
        int pageSize = 20);

    /// <summary>
    /// Gets audit logs for a specific resource.
    /// </summary>
    Task<AuditLogResponse> GetResourceAuditLogsAsync(
        string resourceType,
        int resourceId,
        int page = 1,
        int pageSize = 20);
}
