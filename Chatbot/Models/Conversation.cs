using Chatbot.Models;

namespace Chatbot.Models;

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
