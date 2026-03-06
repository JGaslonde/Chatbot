using Chatbot.Core.Models.Requests;
using Chatbot.Core.Models.Responses;

namespace Chatbot.API.Services.Core.Interfaces;

/// <summary>
/// Service for managing webhooks and external integrations.
/// </summary>
public interface IWebhookService
{
    Task<WebhookDto?> CreateWebhookAsync(int userId, WebhookRequest request);
    Task<List<WebhookDto>> GetUserWebhooksAsync(int userId);
    Task<bool> UpdateWebhookAsync(int userId, int webhookId, WebhookRequest request);
    Task<bool> DeleteWebhookAsync(int userId, int webhookId);
    Task TriggerWebhookAsync(int userId, string eventType, string resourceType, int? resourceId, Dictionary<string, object> data);
    Task ProcessFailedDeliveriesAsync();
}
