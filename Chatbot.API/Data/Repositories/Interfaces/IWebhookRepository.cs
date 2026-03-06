using Chatbot.Core.Models.Entities;

namespace Chatbot.API.Data.Repositories.Interfaces;

public interface IWebhookRepository : IRepository<Webhook>
{
    Task<IEnumerable<Webhook>> GetUserWebhooksAsync(int userId);
    Task<Webhook?> GetWithDeliveriesAsync(int id);
}
