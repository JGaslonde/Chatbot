namespace Chatbot.Core.Models.Entities;

/// <summary>
/// Webhook delivery history.
/// </summary>
public class WebhookDelivery
{
    public int Id { get; set; }
    public int WebhookId { get; set; }
    public WebhookDeliveryStatus Status { get; set; }
    public int? StatusCode { get; set; }
    public string? ResponseBody { get; set; }
    public string? ErrorMessage { get; set; }
    public int AttemptCount { get; set; } = 1;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public Webhook? Webhook { get; set; }
}
