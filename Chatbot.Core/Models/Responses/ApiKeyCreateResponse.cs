namespace Chatbot.Core.Models.Responses;

public record ApiKeyCreateResponse(
    int Id,
    string Key, // Only shown once
    string Name,
    DateTime CreatedAt
);
