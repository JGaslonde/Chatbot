namespace Chatbot.Core.Models.Entities;

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
