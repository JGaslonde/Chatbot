namespace Chatbot.Core.Models.Requests;

public record ChatMessageRequest(string Message, string? ConversationId = null);
