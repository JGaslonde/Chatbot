namespace Chatbot.Core.Models.Entities;

public class User
{
    public int Id { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public string? DisplayName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastActive { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    // Navigation
    public ICollection<Conversation> Conversations { get; set; } = new List<Conversation>();
    public UserPreferences? Preferences { get; set; }
}

public class Conversation
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string? Title { get; set; }
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastMessageAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
    public string? Summary { get; set; }

    // Navigation
    public required User User { get; set; }
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}

public class Message
{
    public int Id { get; set; }
    public int ConversationId { get; set; }
    public required string Content { get; set; }
    public MessageSender Sender { get; set; } = MessageSender.User;
    public DateTime SentAt { get; set; } = DateTime.UtcNow;

    // Analysis results
    public Sentiment Sentiment { get; set; } = Sentiment.Neutral;
    public double SentimentScore { get; set; } = 0.0;
    public string? DetectedIntent { get; set; }
    public double IntentConfidence { get; set; } = 0.0;
    public bool IsFiltered { get; set; } = false;
    public string? FilterReason { get; set; }

    // Navigation
    public required Conversation Conversation { get; set; }
}

public enum MessageSender
{
    User,
    Bot,
    System
}

public enum Sentiment
{
    VeryNegative,
    Negative,
    Neutral,
    Positive,
    VeryPositive
}

public enum UserRole
{
    User,
    Admin,
    Moderator
}

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
