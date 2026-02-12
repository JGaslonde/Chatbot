using Chatbot.Services;

namespace Chatbot;

/// <summary>
/// Core chatbot logic with simple response patterns and advanced features
/// </summary>
public class ChatBot
{
    private readonly Conversation _conversation;
    private readonly Dictionary<string, List<string>> _responsePatterns;
    private readonly Random _random;
    private readonly SentimentAnalyzer _sentimentAnalyzer;
    private readonly IntentRecognizer _intentRecognizer;
    private readonly MessageFilter _messageFilter;

    public string Name { get; set; }

    public ChatBot(string name = "ChatBot")
    {
        Name = name;
        _conversation = new Conversation();
        _random = new Random();
        _responsePatterns = InitializeResponsePatterns();
        _sentimentAnalyzer = new SentimentAnalyzer();
        _intentRecognizer = new IntentRecognizer();
        _messageFilter = new MessageFilter();
    }

    /// <summary>
    /// Process user input and generate a response
    /// </summary>
    public string ProcessMessage(string userInput)
    {
        if (string.IsNullOrWhiteSpace(userInput))
        {
            return "Please say something!";
        }

        // Check message filter
        var filterResult = _messageFilter.CheckMessage(userInput);
        if (filterResult.IsFiltered)
        {
            string reasons = string.Join(", ", filterResult.Reasons);
            return $"Your message cannot be processed: {reasons}";
        }

        // Analyze sentiment
        var sentimentResult = _sentimentAnalyzer.Analyze(userInput);
        
        // Recognize intent
        var intentResult = _intentRecognizer.Recognize(userInput);

        // Store user message with analysis in conversation history
        _conversation.AddMessage("User", userInput, sentimentResult.Sentiment.ToString(), intentResult.Intent);

        // Generate response based on input
        string response = GenerateResponse(userInput, sentimentResult, intentResult);
        
        // Store bot response in conversation history
        _conversation.AddMessage(Name, response);

        return response;
    }

    /// <summary>
    /// Generate a response based on user input and analysis
    /// </summary>
    private string GenerateResponse(string input, SentimentResult sentiment, IntentResult intent)
    {
        string lowerInput = input.ToLower().Trim();

        // Adapt response based on sentiment if very negative
        if (sentiment.Sentiment == Services.Sentiment.VeryNegative)
        {
            return "I sense you're frustrated. I'm here to help. Let me try to address your concern.";
        }

        // Use intent-based routing
        string response = intent.Intent switch
        {
            "greeting" => GetRandomResponse("greeting"),
            "farewell" => GetRandomResponse("farewell"),
            "help" => GetRandomResponse("help"),
            "thanks" => GetRandomResponse("thanks"),
            "question" => GetRandomResponse("question"),
            _ => GenerateDefaultResponse(lowerInput)
        };

        return response;
    }

    /// <summary>
    /// Generate default response when no specific pattern matches
    /// </summary>
    private string GenerateDefaultResponse(string lowerInput)
    {
        // Check for name request
        if (lowerInput.Contains("your name") || lowerInput.Contains("who are you") || lowerInput.Contains("what are you called"))
        {
            return $"I'm {Name}, your friendly chatbot assistant!";
        }

        return GetRandomResponse("default");
    }

    /// <summary>
    /// Get a random response from a category
    /// </summary>
    private string GetRandomResponse(string category)
    {
        if (_responsePatterns.TryGetValue(category, out var responses))
        {
            return responses[_random.Next(responses.Count)];
        }
        return "I'm not sure how to respond to that.";
    }

    /// <summary>
    /// Initialize response patterns for different scenarios
    /// </summary>
    private Dictionary<string, List<string>> InitializeResponsePatterns()
    {
        return new Dictionary<string, List<string>>
        {
            ["greeting"] = new List<string>
            {
                $"Hello! I'm {Name}. How can I help you today?",
                "Hi there! What's on your mind?",
                "Greetings! How may I assist you?",
                "Hey! Nice to chat with you!"
            },
            ["farewell"] = new List<string>
            {
                "Goodbye! Have a great day!",
                "See you later! Take care!",
                "Farewell! Feel free to come back anytime!",
                "Bye! It was nice talking to you!"
            },
            ["help"] = new List<string>
            {
                "I'm here to chat with you! I can respond to greetings, questions, and general conversation. Just type anything!",
                "I can help you with general conversation. Ask me questions or just chat!",
                "Feel free to ask me anything or just have a casual chat!"
            },
            ["question"] = new List<string>
            {
                "That's an interesting question! Let me think... Based on what I know, I'd say that's worth exploring further.",
                "Great question! I'm still learning, but I'll do my best to help.",
                "Hmm, that's something to consider. What are your thoughts on it?",
                "I appreciate your curiosity! Let's explore that together."
            },
            ["thanks"] = new List<string>
            {
                "You're welcome! Happy to help!",
                "No problem at all!",
                "Glad I could assist you!",
                "Anytime! That's what I'm here for!"
            },
            ["default"] = new List<string>
            {
                "I understand. Tell me more!",
                "That's interesting! Can you elaborate?",
                "I see. What else would you like to discuss?",
                "Noted! Anything else on your mind?",
                "I hear you. What else can I help with?"
            }
        };
    }

    /// <summary>
    /// Get conversation history
    /// </summary>
    public Conversation GetConversation()
    {
        return _conversation;
    }

    /// <summary>
    /// Display conversation history
    /// </summary>
    public void ShowHistory(int messageCount = 10)
    {
        var recentMessages = _conversation.GetRecentMessages(messageCount);
        
        Console.WriteLine("\n=== Recent Conversation History ===");
        foreach (var msg in recentMessages)
        {
            string analysis = "";
            if (!string.IsNullOrEmpty(msg.Sentiment) || !string.IsNullOrEmpty(msg.Intent))
            {
                analysis = $" [Sentiment: {msg.Sentiment ?? "N/A"}, Intent: {msg.Intent ?? "N/A"}]";
            }
            Console.WriteLine($"[{msg.Timestamp:HH:mm:ss}] {msg.Sender}: {msg.Content}{analysis}");
        }
        Console.WriteLine("===================================\n");
    }

    /// <summary>
    /// Analyze a message and display the results
    /// </summary>
    public void AnalyzeMessage(string message)
    {
        Console.WriteLine("\n=== Message Analysis ===");
        Console.WriteLine($"Message: {message}");
        
        var sentiment = _sentimentAnalyzer.Analyze(message);
        Console.WriteLine($"Sentiment: {sentiment.Sentiment} (Score: {sentiment.Score:F2})");
        
        var intent = _intentRecognizer.Recognize(message);
        Console.WriteLine($"Intent: {intent.Intent} (Confidence: {intent.Confidence:F2})");
        
        var filter = _messageFilter.CheckMessage(message);
        Console.WriteLine($"Filtered: {filter.IsFiltered}");
        if (filter.IsFiltered && filter.Reasons.Count > 0)
        {
            Console.WriteLine($"Reasons: {string.Join(", ", filter.Reasons)}");
        }
        Console.WriteLine("=======================\n");
    }

    /// <summary>
    /// Clear conversation history
    /// </summary>
    public void ClearHistory()
    {
        _conversation.Clear();
        Console.WriteLine("Conversation history cleared.");
    }
}
