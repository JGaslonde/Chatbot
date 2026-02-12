namespace Chatbot.API.Models.Requests;

public record ChatMessageRequest(string Message, string? ConversationId = null);

public record CreateUserRequest(string Username, string Email, string Password);

public record LoginRequest(string Username, string Password);

public record StartConversationRequest(string? Title = null);
