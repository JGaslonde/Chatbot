using Chatbot.Core.Models.Entities;

namespace Chatbot.API.Data.Repositories.Interfaces;

public interface IWebhookDeliveryRepository : IRepository<WebhookDelivery>
{
    Task<IEnumerable<WebhookDelivery>> GetByWebhookIdAsync(int webhookId);
    Task<IEnumerable<WebhookDelivery>> GetFailedDeliveriesAsync();
}
