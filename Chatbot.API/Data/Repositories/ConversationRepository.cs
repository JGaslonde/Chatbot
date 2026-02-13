using Chatbot.API.Data.Context;
using Chatbot.Core.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Chatbot.API.Data.Repositories;

public class ConversationRepository : Repository<Conversation>, IConversationRepository
{
    public ConversationRepository(ChatbotDbContext context) : base(context) { }

    public async Task<IEnumerable<Conversation>> GetUserConversationsAsync(int userId)
    {
        return await Task.FromResult(
            _context.Conversations
                .Where(c => c.UserId == userId && c.IsActive)
                .OrderByDescending(c => c.LastMessageAt)
                .ToList()
        );
    }

    public async Task<Conversation?> GetWithMessagesAsync(int id)
    {
        return await Task.FromResult(
            _context.Conversations
                .Where(c => c.Id == id)
                .Include(c => c.Messages)
                .FirstOrDefault()
        );
    }
}
