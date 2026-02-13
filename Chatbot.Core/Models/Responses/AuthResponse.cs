namespace Chatbot.Core.Models.Responses;

public record AuthResponse(
    string Token,
    string Username,
    string Email,
    DateTime ExpiresAt
);
