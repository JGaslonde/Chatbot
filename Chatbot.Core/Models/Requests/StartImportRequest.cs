namespace Chatbot.Core.Models.Requests;

public record StartImportRequest(
    string ImportType,
    int FileSize,
    string? FileName = null
);
