using System.ComponentModel.DataAnnotations;

namespace Chatbot.Core.Models.Entities;

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
