namespace Chatbot.Core.Models.Responses;

public record ImportJobDto(
    int Id,
    string FileName,
    string ImportType,
    string Status,
    int TotalRecords,
    int ProcessedRecords,
    int FailedRecords,
    DateTime CreatedAt,
    DateTime? CompletedAt
);
