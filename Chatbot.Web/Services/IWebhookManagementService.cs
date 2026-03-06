using Chatbot.Core.Models.Requests;
using Chatbot.Core.Models.Responses;

namespace Chatbot.Web.Services;

public interface IWebhookManagementService
{
    Task<WebhookDto?> CreateWebhookAsync(WebhookRequest request);
    Task<List<WebhookDto>> GetWebhooksAsync();
    Task<bool> UpdateWebhookAsync(int webhookId, WebhookRequest request);
    Task<bool> DeleteWebhookAsync(int webhookId);
}
