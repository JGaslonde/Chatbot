using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

/// <summary>
/// ML insights extracted from conversation patterns.
/// </summary>
public class MLInsight
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    /// <summary>
    /// Type of insight: TopicCluster, IntentPattern, AnomalyDetection, etc.
    /// </summary>
    [Required]
    public string InsightType { get; set; } = string.Empty;

    /// <summary>
    /// The actual insight value/finding
    /// </summary>
    [Required]
    public string InsightValue { get; set; } = string.Empty;

    /// <summary>
    /// Confidence score (0-1)
    /// </summary>
    public double Confidence { get; set; }

    /// <summary>
    /// Number of conversations this insight is based on
    /// </summary>
    public int SampleSize { get; set; }

    /// <summary>
    /// JSON containing detailed insight data
    /// </summary>
    public string? DetailedData { get; set; }

    /// <summary>
    /// Optional recommended action
    /// </summary>
    public string? RecommendedAction { get; set; }

    [Required]
    public DateTime GeneratedAt { get; set; }

    /// <summary>
    /// Whether this insight has been reviewed/actioned
    /// </summary>
    public bool IsReviewed { get; set; }
}

/// <summary>
/// Workflow automation definitions for multi-step processes.
/// </summary>
public class WorkflowDefinition
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// Trigger condition (e.g., "sentiment < 0.3", "intent == customer_complaint")
    /// </summary>
    [Required]
    public string TriggerCondition { get; set; } = string.Empty;

    /// <summary>
    /// JSON array of workflow steps
    /// </summary>
    [Required]
    public string WorkflowSteps { get; set; } = "[]";

    /// <summary>
    /// Is workflow active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Number of times this workflow has been executed
    /// </summary>
    public int ExecutionCount { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }

    [Required]
    public DateTime UpdatedAt { get; set; }

    public DateTime? LastExecutedAt { get; set; }
}

/// <summary>
/// Execution history of workflows for audit and monitoring.
/// </summary>
public class WorkflowExecution
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int WorkflowDefinitionId { get; set; }

    [Required]
    public int ConversationId { get; set; }

    /// <summary>
    /// Status: Pending, Running, Completed, Failed
    /// </summary>
    [Required]
    public string Status { get; set; } = "Pending";

    /// <summary>
    /// Error message if failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// JSON containing execution context and results
    /// </summary>
    public string? ExecutionContext { get; set; }

    [Required]
    public DateTime StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }
}

/// <summary>
/// User segments based on behavioral analysis.
/// </summary>
public class UserSegment
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    /// <summary>
    /// Engagement level: Low, Medium, High, VeryHigh
    /// </summary>
    [Required]
    public string EngagementLevel { get; set; } = "Medium";

    /// <summary>
    /// Behavioral segment: NewUser, RegularUser, PowerUser, ChurningUser, etc.
    /// </summary>
    [Required]
    public string BehavioralSegment { get; set; } = "RegularUser";

    /// <summary>
    /// Churn risk score (0-1, higher = more likely to churn)
    /// </summary>
    public double ChurnRiskScore { get; set; }

    /// <summary>
    /// Average daily conversation count
    /// </summary>
    public double AverageDailyConversations { get; set; }

    /// <summary>
    /// Average satisfaction score (0-1)
    /// </summary>
    public double AverageSatisfaction { get; set; }

    /// <summary>
    /// Most common use case/intent for this user
    /// </summary>
    public string? PrimaryUseCase { get; set; }

    /// <summary>
    /// Last interaction date
    /// </summary>
    public DateTime? LastInteractionDate { get; set; }

    /// <summary>
    /// Predicted next churn date if at risk
    /// </summary>
    public DateTime? PredictedChurnDate { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }

    [Required]
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Full-text search index entries for conversations.
/// </summary>
public class SearchIndex
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public int ConversationId { get; set; }

    /// <summary>
    /// The searchable content (combined user messages)
    /// </summary>
    [Required]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Extracted keywords from conversation
    /// </summary>
    public string? Keywords { get; set; }

    /// <summary>
    /// Topics detected in conversation
    /// </summary>
    public string? Topics { get; set; }

    /// <summary>
    /// Intents detected in conversation
    /// </summary>
    public string? Intents { get; set; }

    /// <summary>
    /// Full-text search vector (for advanced search engines)
    /// </summary>
    public string? SearchVector { get; set; }

    /// <summary>
    /// Relevance score for search ranking
    /// </summary>
    public double RelevanceScore { get; set; }

    [Required]
    public DateTime IndexedAt { get; set; }

    public DateTime? LastSearchedAt { get; set; }
}
