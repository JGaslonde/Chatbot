namespace Chatbot.Web.Services.Interfaces;

/// <summary>
/// User preferences model
/// </summary>
public class UserPreferences
{
    public int UserId { get; set; }
    public string? Theme { get; set; } = "light";
    public string? Language { get; set; } = "en";
    public bool NotificationsEnabled { get; set; } = true;
    public bool DarkMode { get; set; } = false;
    public int MessagePageSize { get; set; } = 20;
    public DateTime? UpdatedAt { get; set; }
}
