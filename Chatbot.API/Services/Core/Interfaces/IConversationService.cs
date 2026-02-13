using Chatbot.Core.Models.Entities;

namespace Chatbot.API.Services.Core.Interfaces;

public interface IConversationService
{
    Task<Conversation> CreateConversationAsync(int userId, string? title = null);
    Task<Conversation?> GetConversationAsync(int conversationId);
    Task<IEnumerable<Conversation>> GetUserConversationsAsync(int userId);
    Task<Message> AddMessageAsync(int conversationId, string content, MessageSender sender);
    Task<IEnumerable<Message>> GetConversationHistoryAsync(int conversationId);
    Task<bool> UpdateConversationAsync(int conversationId, string? title = null);
    Task<string> GenerateBotResponseAsync(int conversationId, string userMessage);
    Task UpdateConversationSummaryAsync(int conversationId);
}
