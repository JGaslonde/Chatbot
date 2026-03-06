namespace Chatbot.Core.Models.Entities;

/// <summary>
/// IP whitelist for security.
/// </summary>
public class IpWhitelist
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiresAt { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public User? User { get; set; }
}
