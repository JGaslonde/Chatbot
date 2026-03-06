using Chatbot.API.Data.Repositories.Interfaces;
using Chatbot.Core.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Chatbot.API.Data.Repositories;

public class WebhookRepository : Repository<Webhook>, IWebhookRepository
{
    public WebhookRepository(ChatbotDbContext context) : base(context) { }

    public async Task<IEnumerable<Webhook>> GetUserWebhooksAsync(int userId)
    {
        return await _context.Webhooks
            .Where(w => w.UserId == userId)
            .OrderByDescending(w => w.CreatedAt)
            .ToListAsync();
    }

    public async Task<Webhook?> GetWithDeliveriesAsync(int id)
    {
        return await _context.Webhooks
            .Include(w => w.Deliveries)
            .FirstOrDefaultAsync(w => w.Id == id);
    }
}
