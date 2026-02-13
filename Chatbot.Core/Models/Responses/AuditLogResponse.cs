namespace Chatbot.Core.Models.Responses;

/// <summary>
/// Response for audit log entry.
/// </summary>
public record AuditLogEntry(
    int Id,
    int UserId,
    string Action,
    string ResourceType,
    int? ResourceId,
    DateTime Timestamp,
    string IpAddress,
    Dictionary<string, object>? Details
);

/// <summary>
/// Paginated audit log response.
/// </summary>
public record AuditLogResponse(
    List<AuditLogEntry> Logs,
    int TotalCount,
    int Page,
    int PageSize
);
