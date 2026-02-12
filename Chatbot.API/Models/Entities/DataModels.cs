namespace Chatbot.API.Models.Entities;

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
