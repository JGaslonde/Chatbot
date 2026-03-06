namespace Chatbot.API.Models;

public record ApiError(
    string Message,
    string? ErrorCode,
    int? StatusCode
);
