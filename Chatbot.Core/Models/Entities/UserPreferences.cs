namespace Chatbot.Core.Models.Entities;

public class UserPreferences
{
    public int Id { get; set; }
    public int UserId { get; set; }

    // Display preferences
    public string Language { get; set; } = "en";
    public string Theme { get; set; } = "light";
    public string TimeZone { get; set; } = "UTC";

    // Notification preferences
    public bool EmailNotifications { get; set; } = true;
    public bool PushNotifications { get; set; } = true;
    public bool SoundEnabled { get; set; } = true;

    // Conversation preferences
    public string ResponseStyle { get; set; } = "balanced"; // concise, balanced, detailed
    public bool ShowSentimentAnalysis { get; set; } = false;
    public bool ShowIntentRecognition { get; set; } = false;

    // Privacy preferences
    public bool SaveConversationHistory { get; set; } = true;
    public bool AllowDataAnalytics { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public required User User { get; set; }
}
