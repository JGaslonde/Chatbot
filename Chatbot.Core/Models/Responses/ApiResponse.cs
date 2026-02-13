namespace Chatbot.Core.Models.Responses;

public record ApiResponse<T>(
    bool Success,
    string Message,
    T? Data,
    List<string>? Errors = null
);
