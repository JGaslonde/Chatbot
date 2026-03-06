namespace Chatbot.Core.Models.Entities;

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
