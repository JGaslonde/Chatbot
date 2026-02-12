namespace Chatbot.Services;

/// <summary>
/// Result of message filtering
/// </summary>
public class FilterResult
{
    public bool IsFiltered { get; set; }
    public List<string> Reasons { get; set; } = new();
}

/// <summary>
/// Message filtering and moderation service for the console chatbot
/// </summary>
public class MessageFilter
{
    private readonly HashSet<string> _profanityWords;
    private readonly int _maxMessageLength;

    public MessageFilter(int maxMessageLength = 5000)
    {
        _maxMessageLength = maxMessageLength;
        
        // Basic profanity list (can be extended)
        _profanityWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            // Placeholder words - extend with actual profanity list as needed
            "spam", "scam"
        };
    }

    /// <summary>
    /// Check if a message should be filtered
    /// </summary>
    public FilterResult CheckMessage(string message)
    {
        var result = new FilterResult { IsFiltered = false };

        if (string.IsNullOrWhiteSpace(message))
        {
            return result;
        }

        // Check message length
        if (message.Length > _maxMessageLength)
        {
            result.IsFiltered = true;
            result.Reasons.Add($"Message too long (max {_maxMessageLength} characters)");
        }

        // Check for profanity
        string lowerMessage = message.ToLower();
        foreach (var word in _profanityWords)
        {
            if (lowerMessage.Contains(word))
            {
                result.IsFiltered = true;
                result.Reasons.Add("Contains inappropriate language");
                break;
            }
        }

        // Check for excessive special characters
        int specialCharCount = message.Count(c => !char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c));
        double specialCharRatio = (double)specialCharCount / message.Length;
        if (specialCharRatio > 0.5)
        {
            result.IsFiltered = true;
            result.Reasons.Add("Excessive special characters");
        }

        // Check for excessive repetition (e.g., "aaaaaaa")
        for (int i = 0; i < message.Length - 4; i++)
        {
            if (i + 4 < message.Length &&
                message[i] == message[i + 1] &&
                message[i] == message[i + 2] &&
                message[i] == message[i + 3] &&
                message[i] == message[i + 4])
            {
                result.IsFiltered = true;
                result.Reasons.Add("Excessive character repetition");
                break;
            }
        }

        return result;
    }

    /// <summary>
    /// Add a word to the profanity filter
    /// </summary>
    public void AddProfanityWord(string word)
    {
        _profanityWords.Add(word);
    }
}
