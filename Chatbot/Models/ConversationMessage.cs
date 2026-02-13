namespace Chatbot.Models;

/// <summary>
/// Represents a single message in the conversation
/// </summary>
public class ConversationMessage
{
    public string Sender { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string? Sentiment { get; set; }
    public string? Intent { get; set; }

    public ConversationMessage(string sender, string content)
    {
        Sender = sender;
        Content = content;
        Timestamp = DateTime.UtcNow;
    }
}
