namespace Chatbot.Core.Models.Entities;

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
