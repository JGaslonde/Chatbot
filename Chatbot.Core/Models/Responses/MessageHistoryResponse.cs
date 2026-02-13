namespace Chatbot.Core.Models.Responses;

public record MessageHistoryResponse(
    int ConversationId,
    List<MessageDto> Messages
);
