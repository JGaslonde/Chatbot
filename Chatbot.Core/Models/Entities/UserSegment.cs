using System.ComponentModel.DataAnnotations;

namespace Chatbot.Core.Models.Entities;

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
