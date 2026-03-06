using System.ComponentModel.DataAnnotations;

namespace Chatbot.Core.Models.Entities;

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
