namespace Chatbot.Core.Models.Requests;

public record ScheduledReportRequest(
    string Name,
    string? Description,
    string ReportType,
    string Frequency,
    string? RecipientEmail
);
