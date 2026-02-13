namespace Chatbot.Core.Models.Responses;

/// <summary>
/// Response for batch operation results.
/// </summary>
public record BatchOperationResponse(
    bool Success,
    string Message,
    int ProcessedCount,
    int FailedCount,
    List<string> Errors
);
