using System.Security.Cryptography;
using System.Text;
using Chatbot.API.Data.Repositories.Interfaces;
using Chatbot.API.Services.Core.Interfaces;
using Chatbot.API.Hubs;
using Chatbot.Core.Models.Entities;
using Chatbot.Core.Models.Requests;
using Chatbot.Core.Models.Responses;
using Microsoft.AspNetCore.SignalR;

namespace Chatbot.API.Services.Core;

public class WebhookService : IWebhookService
{
    private readonly IWebhookRepository _webhookRepository;
    private readonly IWebhookDeliveryRepository _deliveryRepository;
    private readonly ILogger<WebhookService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHubContext<ChatHub> _hubContext;

    public WebhookService(
        IWebhookRepository webhookRepository,
        IWebhookDeliveryRepository deliveryRepository,
        ILogger<WebhookService> logger,
        IHttpClientFactory httpClientFactory,
        IHubContext<ChatHub> hubContext)
    {
        _webhookRepository = webhookRepository;
        _deliveryRepository = deliveryRepository;
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _hubContext = hubContext;
    }

    public async Task<WebhookDto?> CreateWebhookAsync(int userId, WebhookRequest request)
    {
        try
        {
            var webhook = new Webhook
            {
                UserId = userId,
                Url = request.Url,
                Secret = request.Secret,
                EventType = (WebhookEventType)Enum.Parse(typeof(WebhookEventType), request.EventType),
                IsActive = request.IsActive,
                MaxRetries = request.MaxRetries,
                CreatedAt = DateTime.UtcNow
            };

            await _webhookRepository.AddAsync(webhook);

            _logger.LogInformation($"Webhook created for user {userId}: {webhook.Id}");

            return MapToDto(webhook);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error creating webhook: {ex.Message}");
            throw;
        }
    }

    public async Task<List<WebhookDto>> GetUserWebhooksAsync(int userId)
    {
        var webhooks = await _webhookRepository.GetUserWebhooksAsync(userId);
        return webhooks.Select(MapToDto).ToList();
    }

    public async Task<bool> UpdateWebhookAsync(int userId, int webhookId, WebhookRequest request)
    {
        var webhook = await _webhookRepository.GetByIdAsync(webhookId);
        if (webhook == null || webhook.UserId != userId)
            return false;

        webhook.Url = request.Url;
        webhook.Secret = request.Secret;
        webhook.EventType = (WebhookEventType)Enum.Parse(typeof(WebhookEventType), request.EventType);
        webhook.IsActive = request.IsActive;
        webhook.MaxRetries = request.MaxRetries;

        await _webhookRepository.UpdateAsync(webhook);

        return true;
    }

    public async Task<bool> DeleteWebhookAsync(int userId, int webhookId)
    {
        var webhook = await _webhookRepository.GetByIdAsync(webhookId);
        if (webhook == null || webhook.UserId != userId)
            return false;

        await _webhookRepository.DeleteAsync(webhookId);

        return true;
    }

    public async Task TriggerWebhookAsync(int userId, string eventType, string resourceType, int? resourceId, Dictionary<string, object> data)
    {
        var webhooks = await _webhookRepository.GetUserWebhooksAsync(userId);
        var applicableWebhooks = webhooks.Where(w => w.IsActive && w.EventType.ToString() == eventType).ToList();

        foreach (var webhook in applicableWebhooks)
        {
            _ = DeliverWebhookAsync(webhook, eventType, resourceType, resourceId, data);
        }
    }

    private async Task DeliverWebhookAsync(Webhook webhook, string eventType, string resourceType, int? resourceId, Dictionary<string, object> data)
    {
        try
        {
            var eventId = Guid.NewGuid().ToString();
            var payload = new WebhookEventPayload(
                eventType,
                eventId,
                DateTime.UtcNow,
                webhook.UserId,
                resourceType,
                resourceId,
                data
            );

            var json = System.Text.Json.JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Add signature
            if (!string.IsNullOrEmpty(webhook.Secret))
            {
                var signature = GenerateSignature(json, webhook.Secret);
                content.Headers.Add("X-Webhook-Signature", signature);
            }

            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsync(webhook.Url, content);

            var delivery = new WebhookDelivery
            {
                WebhookId = webhook.Id,
                Status = response.IsSuccessStatusCode ? WebhookDeliveryStatus.Delivered : WebhookDeliveryStatus.Failed,
                StatusCode = (int)response.StatusCode,
                ResponseBody = await response.Content.ReadAsStringAsync(),
                AttemptCount = 1,
                CreatedAt = DateTime.UtcNow
            };

            await _deliveryRepository.AddAsync(delivery);

            // Broadcast webhook delivery status via SignalR
            await _hubContext.Clients.Group($"webhook_{webhook.Id}")
                .SendAsync("WebhookDelivered", new
                {
                    WebhookId = webhook.Id,
                    EventType = eventType,
                    Success = response.IsSuccessStatusCode,
                    ErrorMessage = response.IsSuccessStatusCode ? null : $"HTTP {response.StatusCode}",
                    Timestamp = DateTime.UtcNow
                });

            if (response.IsSuccessStatusCode)
            {
                webhook.LastTriggeredAt = DateTime.UtcNow;
                await _webhookRepository.UpdateAsync(webhook);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Webhook delivery failed for webhook {webhook.Id}: {ex.Message}");
        }
    }

    public async Task ProcessFailedDeliveriesAsync()
    {
        var failedDeliveries = await _deliveryRepository.GetFailedDeliveriesAsync();

        foreach (var delivery in failedDeliveries)
        {
            var webhook = await _webhookRepository.GetByIdAsync(delivery.WebhookId);
            if (webhook == null) continue;

            delivery.AttemptCount++;
            await _deliveryRepository.UpdateAsync(delivery);
        }
    }

    private static string GenerateSignature(string payload, string secret)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        return Convert.ToBase64String(hash);
    }

    private static WebhookDto MapToDto(Webhook webhook)
    {
        return new WebhookDto(
            webhook.Id,
            webhook.Url,
            webhook.EventType.ToString(),
            webhook.IsActive,
            webhook.CreatedAt,
            webhook.LastTriggeredAt
        );
    }
}
