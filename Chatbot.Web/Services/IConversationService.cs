using Chatbot.Core.Models.Responses;

namespace Chatbot.Web.Services;

public interface IConversationService
{
    Task<(bool Success, string Message, List<ConversationResponse>? Conversations)> GetConversationsAsync();
    Task<(bool Success, string Message, ConversationResponse? Conversation)> GetConversationAsync(int id);
    Task<(bool Success, string Message, byte[]? Data)> ExportConversationAsJsonAsync(int id);
    Task<(bool Success, string Message, byte[]? Data)> ExportConversationAsCsvAsync(int id);
}
