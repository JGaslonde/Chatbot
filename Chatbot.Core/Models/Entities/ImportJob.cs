namespace Chatbot.Core.Models.Entities;

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
