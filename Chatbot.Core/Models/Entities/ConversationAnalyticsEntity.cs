using System.ComponentModel.DataAnnotations;

namespace Chatbot.Core.Models.Entities;

/// <summary>
/// Stores aggregated analytics data for conversations and messages.
/// </summary>
public class ConversationAnalyticsEntity
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public int ConversationId { get; set; }

    /// <summary>
    /// Average sentiment score (0-1, higher = more positive)
    /// </summary>
    public double AverageSentimentScore { get; set; }

    /// <summary>
    /// Total number of messages in conversation
    /// </summary>
    public int TotalMessages { get; set; }

    /// <summary>
    /// Number of user messages
    /// </summary>
    public int UserMessageCount { get; set; }

    /// <summary>
    /// Number of bot messages
    /// </summary>
    public int BotMessageCount { get; set; }

    /// <summary>
    /// Average response time in seconds (bot to user message)
    /// </summary>
    public double AverageResponseTime { get; set; }

    /// <summary>
    /// Average message length
    /// </summary>
    public double AverageMessageLength { get; set; }

    /// <summary>
    /// Most detected intent in conversation
    /// </summary>
    public string? DominantIntent { get; set; }

    /// <summary>
    /// Comma-separated list of detected topics
    /// </summary>
    public string? DetectedTopics { get; set; }

    /// <summary>
    /// Engagement score (0-100) based on interaction patterns
    /// </summary>
    public int EngagementScore { get; set; }

    /// <summary>
    /// Whether conversation was satisfactory (user rated positively)
    /// </summary>
    public bool? IsSatisfactory { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }

    [Required]
    public DateTime UpdatedAt { get; set; }
}
