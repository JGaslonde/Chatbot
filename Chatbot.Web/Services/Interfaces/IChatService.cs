using Chatbot.Core.Models.Responses;

namespace Chatbot.Web.Services.Interfaces;

public interface IChatService
{
    Task<(bool Success, string Message, ConversationResponse? Conversation)> StartConversationAsync(string? title = null);
    Task<(bool Success, string Message, ChatMessageResponse? Response)> SendMessageAsync(int conversationId, string message);
    Task<(bool Success, string Message, MessageHistoryResponse? History)> GetHistoryAsync(int conversationId);
}
