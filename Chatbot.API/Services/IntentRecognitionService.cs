namespace Chatbot.API.Services;

public interface IIntentRecognitionService
{
    Task<(string Intent, double Confidence)> RecognizeIntentAsync(string text);
}

public class SimpleIntentRecognitionService : IIntentRecognitionService
{
    private readonly Dictionary<string, List<string>> _intentPatterns = new()
    {
        {
            "greeting",
            new List<string>
            {
                "hello", "hi", "hey", "good morning", "good afternoon", "good evening",
                "howdy", "greetings", "welcome"
            }
        },
        {
            "farewell",
            new List<string>
            {
                "bye", "goodbye", "see you", "until next time", "take care", "farewell",
                "catch you", "later", "ciao", "adios"
            }
        },
        {
            "help",
            new List<string>
            {
                "help", "assist", "support", "guidance", "how do i", "how can i", "what can i",
                "can you help", "need help", "stuck", "don't know"
            }
        },
        {
            "question",
            new List<string>
            {
                "what", "when", "where", "why", "how", "who", "which", "can you", "will you",
                "do you", "does", "is", "are", "?"
            }
        },
        {
            "command",
            new List<string>
            {
                "do", "create", "make", "send", "get", "find", "show", "list", "generate",
                "start", "stop", "begin", "end", "run", "execute"
            }
        },
        {
            "feedback",
            new List<string>
            {
                "feedback", "suggestion", "comment", "think", "feel", "opinion", "review",
                "rate", "rating", "improvement", "better"
            }
        }
    };

    public async Task<(string Intent, double Confidence)> RecognizeIntentAsync(string text)
    {
        var lower = text.ToLower();
        var words = lower.Split(new[] { ' ', ',', '.', '!', '?', ':' }, StringSplitOptions.RemoveEmptyEntries);
        
        var intentScores = new Dictionary<string, int>();

        foreach (var intent in _intentPatterns.Keys)
        {
            intentScores[intent] = 0;
        }

        foreach (var word in words)
        {
            foreach (var (intent, patterns) in _intentPatterns)
            {
                if (patterns.Any(p => p.Contains(word) || word.Contains(p)))
                {
                    intentScores[intent]++;
                }
            }
        }

        var maxScore = intentScores.Values.DefaultIfEmpty(0).Max();
        if (maxScore == 0)
            return ("unknown", 0.0);

        var topIntent = intentScores.FirstOrDefault(x => x.Value == maxScore).Key;
        var confidence = Math.Min(maxScore / (double)words.Length, 1.0);

        return (topIntent, confidence);
    }
}
