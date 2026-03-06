namespace Chatbot.API.Models.Responses;

/// <summary>
/// User activity log entry
/// </summary>
public class ActivityLogDto
{
    /// <summary>
    /// Activity log ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// User ID who performed the action
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// User name
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// Activity type (created, updated, deleted, viewed, exported, etc.)
    /// </summary>
    public string ActivityType { get; set; } = string.Empty;

    /// <summary>
    /// Resource type (conversation, report, settings, etc.)
    /// </summary>
    public string ResourceType { get; set; } = string.Empty;

    /// <summary>
    /// Resource ID
    /// </summary>
    public int? ResourceId { get; set; }

    /// <summary>
    /// Description of the activity
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// IP address of origin
    /// </summary>
    public string? IpAddress { get; set; }

    /// <summary>
    /// When activity occurred
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Previous values (for audit trail)
    /// </summary>
    public Dictionary<string, object>? PreviousValues { get; set; }

    /// <summary>
    /// New values (for audit trail)
    /// </summary>
    public Dictionary<string, object>? NewValues { get; set; }

    /// <summary>
    /// Success status
    /// </summary>
    public bool IsSuccessful { get; set; } = true;

    /// <summary>
    /// Error message if failed
    /// </summary>
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// User behavior summary
/// </summary>
public class UserActivitySummaryDto
{
    /// <summary>
    /// User ID
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// User name
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Total actions performed in period
    /// </summary>
    public int TotalActions { get; set; }

    /// <summary>
    /// Conversations created
    /// </summary>
    public int ConversationsCreated { get; set; }

    /// <summary>
    /// Messages sent
    /// </summary>
    public int MessagesSent { get; set; }

    /// <summary>
    /// Reports generated
    /// </summary>
    public int ReportsGenerated { get; set; }

    /// <summary>
    /// Data exports performed
    /// </summary>
    public int ExportsPerformed { get; set; }

    /// <summary>
    /// Failed actions
    /// </summary>
    public int FailedActions { get; set; }

    /// <summary>
    /// Last activity time
    /// </summary>
    public DateTime LastActivityAt { get; set; }

    /// <summary>
    /// First activity time in period
    /// </summary>
    public DateTime PeriodStart { get; set; }

    /// <summary>
    /// Last activity time in period
    /// </summary>
    public DateTime PeriodEnd { get; set; }

    /// <summary>
    /// Most common activity type
    /// </summary>
    public string? MostCommonActivity { get; set; }

    /// <summary>
    /// Activity breakdown by type
    /// </summary>
    public Dictionary<string, int> ActivityByType { get; set; } = new();
}
