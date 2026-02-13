using Chatbot.Core.Models.Entities;

namespace Chatbot.API.Data.Repositories.Interfaces;

public interface IMessageRepository : IRepository<Message>
{
    Task<IEnumerable<Message>> GetConversationMessagesAsync(int conversationId);
}
