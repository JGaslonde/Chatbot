using Chatbot.Core.Models.Entities;

namespace Chatbot.API.Data.Repositories;

public interface IConversationRepository : IRepository<Conversation>
{
    Task<IEnumerable<Conversation>> GetUserConversationsAsync(int userId);
    Task<Conversation?> GetWithMessagesAsync(int id);
}
