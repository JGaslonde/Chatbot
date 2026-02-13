using Chatbot.API.Data.Context;
using Chatbot.Core.Models.Entities;

namespace Chatbot.API.Data.Repositories;

public class MessageRepository : Repository<Message>, IMessageRepository
{
    public MessageRepository(ChatbotDbContext context) : base(context) { }

    public async Task<IEnumerable<Message>> GetConversationMessagesAsync(int conversationId)
    {
        return await Task.FromResult(
            _context.Messages
                .Where(m => m.ConversationId == conversationId)
                .OrderBy(m => m.SentAt)
                .ToList()
        );
    }
}
