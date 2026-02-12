namespace Chatbot;

/// <summary>
/// Core chatbot logic with simple response patterns
/// </summary>
public class ChatBot
{
    private readonly Conversation _conversation;
    private readonly Dictionary<string, List<string>> _responsePatterns;
    private readonly Random _random;

    public string Name { get; set; }

    public ChatBot(string name = "ChatBot")
    {
        Name = name;
        _conversation = new Conversation();
        _random = new Random();
        _responsePatterns = InitializeResponsePatterns();
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

        // Store user message in conversation history
        _conversation.AddMessage("User", userInput);

        // Generate response based on input
        string response = GenerateResponse(userInput);
        
        // Store bot response in conversation history
        _conversation.AddMessage(Name, response);

        return response;
    }

    /// <summary>
    /// Generate a response based on user input
    /// </summary>
    private string GenerateResponse(string input)
    {
        string lowerInput = input.ToLower().Trim();

        // Check for greeting patterns
        if (MatchesPattern(lowerInput, new[] { "hello", "hi", "hey", "greetings", "good morning", "good afternoon", "good evening" }))
        {
            return GetRandomResponse("greeting");
        }

        // Check for farewell patterns
        if (MatchesPattern(lowerInput, new[] { "bye", "goodbye", "see you", "farewell", "exit", "quit" }))
        {
            return GetRandomResponse("farewell");
        }

        // Check for help patterns
        if (MatchesPattern(lowerInput, new[] { "help", "what can you do", "how do i", "assist", "support" }))
        {
            return GetRandomResponse("help");
        }

        // Check for question patterns
        if (lowerInput.Contains("?") || MatchesPattern(lowerInput, new[] { "what", "when", "where", "why", "how", "who" }))
        {
            return GetRandomResponse("question");
        }

        // Check for thanks patterns
        if (MatchesPattern(lowerInput, new[] { "thank", "thanks", "appreciate" }))
        {
            return GetRandomResponse("thanks");
        }

        // Check for name request
        if (MatchesPattern(lowerInput, new[] { "your name", "who are you", "what are you called" }))
        {
            return $"I'm {Name}, your friendly chatbot assistant!";
        }

        // Default response
        return GetRandomResponse("default");
    }

    /// <summary>
    /// Check if input matches any of the given patterns
    /// </summary>
    private bool MatchesPattern(string input, string[] patterns)
    {
        return patterns.Any(pattern => input.Contains(pattern));
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
            Console.WriteLine($"[{msg.Timestamp:HH:mm:ss}] {msg.Sender}: {msg.Content}");
        }
        Console.WriteLine("===================================\n");
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
