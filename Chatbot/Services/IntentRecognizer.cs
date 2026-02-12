namespace Chatbot.Services;

/// <summary>
/// Detected user intent
/// </summary>
public class IntentResult
{
    public string Intent { get; set; } = "unknown";
    public double Confidence { get; set; } // 0.0 to 1.0
}

/// <summary>
/// Simple intent recognition service for the console chatbot
/// </summary>
public class IntentRecognizer
{
    private readonly Dictionary<string, string[]> _intentPatterns;

    public IntentRecognizer()
    {
        _intentPatterns = new Dictionary<string, string[]>
        {
            ["greeting"] = new[] { "hello", "hi", "hey", "greetings", "good morning", "good afternoon", "good evening", "howdy", "welcome" },
            ["farewell"] = new[] { "bye", "goodbye", "see you", "farewell", "take care", "later", "exit", "quit" },
            ["help"] = new[] { "help", "assist", "support", "how do i", "can you help", "need help", "what can you do", "guide" },
            ["question"] = new[] { "what", "when", "where", "why", "how", "who", "which", "can", "could", "would", "?" },
            ["command"] = new[] { "do", "create", "make", "start", "stop", "run", "execute", "perform", "build", "delete" },
            ["feedback"] = new[] { "feedback", "suggestion", "comment", "review", "rate", "opinion", "think about" },
            ["thanks"] = new[] { "thank", "thanks", "appreciate", "grateful" }
        };
    }

    /// <summary>
    /// Recognize the intent from a user message
    /// </summary>
    public IntentResult Recognize(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return new IntentResult { Intent = "unknown", Confidence = 0.0 };
        }

        string lowerMessage = message.ToLower();
        
        // Track best match
        string bestIntent = "unknown";
        double bestConfidence = 0.0;

        foreach (var (intent, patterns) in _intentPatterns)
        {
            int matchCount = 0;
            foreach (var pattern in patterns)
            {
                if (lowerMessage.Contains(pattern))
                {
                    matchCount++;
                }
            }

            if (matchCount > 0)
            {
                // Calculate confidence based on pattern matches
                double confidence = Math.Min(1.0, (double)matchCount / patterns.Length + 0.5);
                
                if (confidence > bestConfidence)
                {
                    bestIntent = intent;
                    bestConfidence = confidence;
                }
            }
        }

        return new IntentResult { Intent = bestIntent, Confidence = bestConfidence };
    }
}
