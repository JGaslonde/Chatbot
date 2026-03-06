using System.ComponentModel.DataAnnotations;

namespace Chatbot.Core.Models.Entities;

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
