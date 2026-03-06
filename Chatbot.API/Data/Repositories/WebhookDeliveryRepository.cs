using Chatbot.API.Data.Repositories.Interfaces;
using Chatbot.Core.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Chatbot.API.Data.Repositories;

public class WebhookDeliveryRepository : Repository<WebhookDelivery>, IWebhookDeliveryRepository
{
    public WebhookDeliveryRepository(ChatbotDbContext context) : base(context) { }

    public async Task<IEnumerable<WebhookDelivery>> GetByWebhookIdAsync(int webhookId)
    {
        return await _context.WebhookDeliveries
            .Where(wd => wd.WebhookId == webhookId)
            .OrderByDescending(wd => wd.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<WebhookDelivery>> GetFailedDeliveriesAsync()
    {
        return await _context.WebhookDeliveries
            .Where(wd => wd.Status == WebhookDeliveryStatus.Failed && wd.AttemptCount < 3)
            .OrderBy(wd => wd.CreatedAt)
            .ToListAsync();
    }
}
