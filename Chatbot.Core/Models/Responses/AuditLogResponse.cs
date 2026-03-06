namespace Chatbot.Core.Models.Responses;

/// <summary>
/// Paginated audit log response.
/// </summary>
public record AuditLogResponse(
    List<AuditLogEntry> Logs,
    int TotalCount,
    int Page,
    int PageSize
);
