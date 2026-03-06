namespace Chatbot.Core.Models.Entities;

/// <summary>
/// Webhook delivery status.
/// </summary>
public enum WebhookDeliveryStatus
{
    Pending,
    Delivered,
    Failed,
    Retrying
}
