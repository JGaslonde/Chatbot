namespace Chatbot.Core.Models.Requests;

public record ApiKeyRequest(
    string Name,
    string? Description,
    int? ExpirationDays = null
);
