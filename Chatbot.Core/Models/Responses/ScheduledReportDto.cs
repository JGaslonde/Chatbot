namespace Chatbot.Core.Models.Responses;

public record ScheduledReportDto(
    int Id,
    string Name,
    string? Description,
    string ReportType,
    string Frequency,
    string? RecipientEmail,
    bool IsActive,
    DateTime? LastGeneratedAt,
    DateTime? NextGeneratedAt
);
