namespace Chatbot.Core.Models.Responses;

public record ConversationResponse(
    int Id,
    string? Title,
    DateTime StartedAt,
    int MessageCount,
    string? Summary
);
