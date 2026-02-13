namespace Chatbot.Core.Models.Entities;

/// <summary>
/// Webhook event types.
/// </summary>
public enum WebhookEventType
{
    ConversationStarted,
    ConversationEnded,
    MessageReceived,
    MessageAnalyzed,
    UserRegistered,
    UserDeleted,
    ConversationArchived,
    ConversationDeleted
}

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

/// <summary>
/// API key for programmatic access.
/// </summary>
public class ApiKey
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string KeyHash { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiresAt { get; set; }
    public DateTime? LastUsedAt { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public User? User { get; set; }
}

/// <summary>
/// Two-factor authentication settings.
/// </summary>
public class TwoFactorAuth
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public bool IsEnabled { get; set; } = false;
    public string? Secret { get; set; }
    public DateTime? EnabledAt { get; set; }
    public List<string>? BackupCodes { get; set; }

    // Navigation
    public User? User { get; set; }
}

/// <summary>
/// IP whitelist for security.
/// </summary>
public class IpWhitelist
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiresAt { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public User? User { get; set; }
}

/// <summary>
/// Scheduled report definition.
/// </summary>
public class ScheduledReport
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ReportType { get; set; } = string.Empty; // "analytics", "activity", "sentiment"
    public string Frequency { get; set; } = "weekly"; // "daily", "weekly", "monthly"
    public string? RecipientEmail { get; set; }
    public DateTime? LastGeneratedAt { get; set; }
    public DateTime? NextGeneratedAt { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public User? User { get; set; }
}

/// <summary>
/// Import job for bulk operations.
/// </summary>
public class ImportJob
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ImportType { get; set; } = string.Empty; // "conversations", "messages"
    public string Status { get; set; } = "pending"; // "pending", "processing", "completed", "failed"
    public int TotalRecords { get; set; }
    public int ProcessedRecords { get; set; }
    public int FailedRecords { get; set; }
    public string? ErrorDetails { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }

    // Navigation
    public User? User { get; set; }
}
