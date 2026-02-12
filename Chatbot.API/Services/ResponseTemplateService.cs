using Chatbot.Core.Models.Entities;

namespace Chatbot.API.Services;

public interface IResponseTemplateService
{
    string GenerateResponse(string userMessage, string intent, Sentiment sentiment, double sentimentScore);
    string GenerateContextAwareResponse(string userMessage, List<Message> recentMessages, string intent, Sentiment sentiment);
}

public class ResponseTemplateService : IResponseTemplateService
{
    private readonly Dictionary<string, List<string>> _intentTemplates;
    private readonly Dictionary<Sentiment, List<string>> _sentimentTemplates;
    private readonly Random _random;

    public ResponseTemplateService()
    {
        _random = new Random();
        _intentTemplates = InitializeIntentTemplates();
        _sentimentTemplates = InitializeSentimentTemplates();
    }

    public string GenerateResponse(string userMessage, string intent, Sentiment sentiment, double sentimentScore)
    {
        // Priority 1: Handle very negative sentiment
        if (sentiment == Sentiment.VeryNegative)
        {
            return GetRandomTemplate(_sentimentTemplates[Sentiment.VeryNegative]);
        }

        // Priority 2: Handle very positive sentiment
        if (sentiment == Sentiment.VeryPositive)
        {
            return GetRandomTemplate(_sentimentTemplates[Sentiment.VeryPositive]);
        }

        // Priority 3: Handle based on intent
        if (_intentTemplates.ContainsKey(intent))
        {
            return GetRandomTemplate(_intentTemplates[intent]);
        }

        // Priority 4: Handle based on sentiment
        if (_sentimentTemplates.ContainsKey(sentiment))
        {
            return GetRandomTemplate(_sentimentTemplates[sentiment]);
        }

        // Fallback: Generic response
        return GetRandomTemplate(_intentTemplates["default"]);
    }

    public string GenerateContextAwareResponse(string userMessage, List<Message> recentMessages, string intent, Sentiment sentiment)
    {
        // Check for conversation patterns
        if (recentMessages.Count >= 2)
        {
            var lastTwoMessages = recentMessages.TakeLast(2).ToList();
            
            // Check if user is repeatedly asking similar questions
            if (lastTwoMessages.All(m => m.Sender == MessageSender.User && 
                                        m.DetectedIntent == intent && 
                                        intent == "question"))
            {
                return "I notice you have multiple questions. Let me help you systematically. What's your main concern?";
            }

            // Check if user sentiment has changed dramatically
            if (recentMessages.Count >= 3)
            {
                var previousSentiment = recentMessages[^2].Sentiment;
                if (previousSentiment == Sentiment.Positive && sentiment == Sentiment.VeryNegative)
                {
                    return "I sense something changed. Is everything okay? How can I help?";
                }
            }

            // Check for greeting after farewell (user came back)
            if (recentMessages.Any(m => m.DetectedIntent == "farewell") && intent == "greeting")
            {
                return "Welcome back! How can I assist you further?";
            }
        }

        // Default to standard response generation
        return GenerateResponse(userMessage, intent, sentiment, 0);
    }

    private string GetRandomTemplate(List<string> templates)
    {
        if (templates == null || templates.Count == 0)
            return "I understand. How can I help you further?";
        
        return templates[_random.Next(templates.Count)];
    }

    private Dictionary<string, List<string>> InitializeIntentTemplates()
    {
        return new Dictionary<string, List<string>>
        {
            ["greeting"] = new List<string>
            {
                "Hello! I'm here to help. What can I do for you today?",
                "Hi there! How may I assist you?",
                "Greetings! What brings you here today?",
                "Hey! Nice to meet you. What's on your mind?"
            },
            ["farewell"] = new List<string>
            {
                "Goodbye! Feel free to return anytime you need assistance.",
                "Take care! I'm here whenever you need me.",
                "Farewell! Have a wonderful day!",
                "See you later! Don't hesitate to come back if you need help."
            },
            ["help"] = new List<string>
            {
                "I'm here to assist you! I can answer questions, provide information, and have conversations. What do you need help with?",
                "I'd be happy to help! You can ask me questions, discuss topics, or just chat. What interests you?",
                "Let me help you! I can provide information, answer questions, and engage in conversation. What would you like to know?",
                "I'm at your service! Feel free to ask me anything or discuss any topic you'd like."
            },
            ["question"] = new List<string>
            {
                "That's a great question! Let me think about that for you.",
                "Interesting question! Based on what I know, I'd say that's worth exploring.",
                "Good question! Let me help you with that.",
                "I appreciate your curiosity! Let's explore that together."
            },
            ["command"] = new List<string>
            {
                "I understand you'd like me to do something. Could you provide more details?",
                "I'll do my best to help with that. What specifically would you like?",
                "Got it! Let me see what I can do for you.",
                "I'm on it! Can you give me a bit more context?"
            },
            ["feedback"] = new List<string>
            {
                "Thank you for your feedback! It helps me improve.",
                "I appreciate your input! Your feedback is valuable.",
                "Thanks for sharing that with me! I'll keep that in mind.",
                "Your feedback matters! Thank you for taking the time to share."
            },
            ["thanks"] = new List<string>
            {
                "You're very welcome! Happy to help!",
                "No problem at all! That's what I'm here for.",
                "Glad I could assist! Feel free to ask anything else.",
                "Anytime! I'm always here to help."
            },
            ["default"] = new List<string>
            {
                "I understand. Tell me more about what you're thinking.",
                "That's interesting! Can you elaborate?",
                "I see. What else is on your mind?",
                "Noted! How else can I assist you?",
                "I hear you. What would you like to discuss further?"
            }
        };
    }

    private Dictionary<Sentiment, List<string>> InitializeSentimentTemplates()
    {
        return new Dictionary<Sentiment, List<string>>
        {
            [Sentiment.VeryNegative] = new List<string>
            {
                "I sense you're frustrated. I'm here to help. Let me try to address your concern.",
                "I understand you're upset. Please tell me more so I can assist you better.",
                "I'm sorry you're having a difficult time. How can I help make this better?",
                "I hear your frustration. Let's work together to resolve this."
            },
            [Sentiment.Negative] = new List<string>
            {
                "I understand this might not be ideal. How can I help improve the situation?",
                "I see this is concerning you. Let me try to help.",
                "I hear you. What can I do to assist?"
            },
            [Sentiment.VeryPositive] = new List<string>
            {
                "That's wonderful! I'm so glad to hear that!",
                "Fantastic! Your enthusiasm is contagious!",
                "That's amazing! I'm thrilled things are going well!",
                "Excellent! It's great to see you so happy!"
            },
            [Sentiment.Positive] = new List<string>
            {
                "That's great to hear! How can I help you further?",
                "Wonderful! What else can I do for you?",
                "I'm glad to hear that! What's next?"
            }
        };
    }
}
