namespace Chatbot.Core.Models.Entities;

/// <summary>
/// Two-factor authentication settings.
/// </summary>
public class TwoFactorAuth
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public bool IsEnabled { get; set; } = false;
    public string? Secret { get; set; }
    public DateTime? EnabledAt { get; set; }
    public List<string>? BackupCodes { get; set; }

    // Navigation
    public User? User { get; set; }
}
