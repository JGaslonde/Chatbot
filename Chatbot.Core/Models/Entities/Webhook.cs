namespace Chatbot.Core.Models.Entities;

/// <summary>
/// Webhook definition for external integrations.
/// </summary>
public class Webhook
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Url { get; set; } = string.Empty;
    public string? Secret { get; set; }
    public WebhookEventType EventType { get; set; }
    public bool IsActive { get; set; } = true;
    public int MaxRetries { get; set; } = 3;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastTriggeredAt { get; set; }

    // Navigation
    public User? User { get; set; }
    public ICollection<WebhookDelivery> Deliveries { get; set; } = new List<WebhookDelivery>();
}
