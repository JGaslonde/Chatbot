using Chatbot.Core.Models.Entities;

namespace Chatbot.API.Data.Repositories.Interfaces;

public interface IConversationRepository : IRepository<Conversation>
{
    Task<IEnumerable<Conversation>> GetUserConversationsAsync(int userId);
    Task<Conversation?> GetWithMessagesAsync(int id);
}
