namespace Chatbot;

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

/// <summary>
/// Manages conversation history and context
/// </summary>
public class Conversation
{
    private readonly List<ConversationMessage> _messages;
    private readonly int _maxHistorySize;

    public IReadOnlyList<ConversationMessage> Messages => _messages.AsReadOnly();
    public DateTime StartedAt { get; }
    public int MessageCount => _messages.Count;

    public Conversation(int maxHistorySize = 100)
    {
        _messages = new List<ConversationMessage>();
        _maxHistorySize = maxHistorySize;
        StartedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Adds a message to the conversation history
    /// </summary>
    public void AddMessage(string sender, string content, string? sentiment = null, string? intent = null)
    {
        var message = new ConversationMessage(sender, content)
        {
            Sentiment = sentiment,
            Intent = intent
        };
        
        _messages.Add(message);

        // Keep history size manageable
        if (_messages.Count > _maxHistorySize)
        {
            _messages.RemoveAt(0);
        }
    }

    /// <summary>
    /// Gets the most recent messages
    /// </summary>
    public IEnumerable<ConversationMessage> GetRecentMessages(int count)
    {
        return _messages.TakeLast(count);
    }

    /// <summary>
    /// Clears conversation history
    /// </summary>
    public void Clear()
    {
        _messages.Clear();
    }

    /// <summary>
    /// Gets a summary of the conversation
    /// </summary>
    public string GetSummary()
    {
        return $"Conversation started at {StartedAt:yyyy-MM-dd HH:mm:ss} with {MessageCount} messages.";
    }
}
