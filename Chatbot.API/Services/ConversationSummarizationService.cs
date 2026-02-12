using Chatbot.API.Models.Entities;

namespace Chatbot.API.Services;

public interface IConversationSummarizationService
{
    string GenerateSummary(List<Message> messages);
    string GenerateTitle(List<Message> messages);
}

public class ConversationSummarizationService : IConversationSummarizationService
{
    public string GenerateSummary(List<Message> messages)
    {
        if (messages == null || messages.Count == 0)
            return "Empty conversation";

        var userMessages = messages.Where(m => m.Sender == MessageSender.User).ToList();
        if (userMessages.Count == 0)
            return "No user messages";

        // Extract key statistics
        int totalMessages = messages.Count;
        int userMessageCount = userMessages.Count;
        int botMessageCount = messages.Count(m => m.Sender == MessageSender.Bot);

        // Analyze intents
        var intentCounts = userMessages
            .Where(m => !string.IsNullOrEmpty(m.DetectedIntent))
            .GroupBy(m => m.DetectedIntent)
            .OrderByDescending(g => g.Count())
            .Take(3)
            .Select(g => $"{g.Key} ({g.Count()})")
            .ToList();

        // Analyze sentiment trend
        var sentiments = userMessages
            .Select(m => m.Sentiment)
            .ToList();

        var avgSentiment = CalculateAverageSentiment(sentiments);
        string sentimentTrend = GetSentimentDescription(avgSentiment);

        // Detect topics (simple keyword extraction from first few messages)
        var keywords = ExtractKeywords(userMessages.Take(5).Select(m => m.Content).ToList());
        string topicsStr = keywords.Any() ? string.Join(", ", keywords.Take(3)) : "general chat";

        // Build summary
        var summary = $"Conversation with {totalMessages} messages ({userMessageCount} from user, {botMessageCount} from bot). ";
        
        if (intentCounts.Any())
            summary += $"Main intents: {string.Join(", ", intentCounts)}. ";
        
        summary += $"Overall sentiment: {sentimentTrend}. ";
        summary += $"Topics discussed: {topicsStr}.";

        return summary;
    }

    public string GenerateTitle(List<Message> messages)
    {
        if (messages == null || messages.Count == 0)
            return "New Conversation";

        var userMessages = messages.Where(m => m.Sender == MessageSender.User).ToList();
        if (userMessages.Count == 0)
            return "New Conversation";

        // Try to extract title from first user message or dominant intent
        var firstMessage = userMessages.First();
        var dominantIntent = userMessages
            .Where(m => !string.IsNullOrEmpty(m.DetectedIntent))
            .GroupBy(m => m.DetectedIntent)
            .OrderByDescending(g => g.Count())
            .FirstOrDefault()?.Key;

        // Generate title based on intent or content
        if (!string.IsNullOrEmpty(dominantIntent))
        {
            return dominantIntent switch
            {
                "help" => "Help Request",
                "question" => ExtractQuestionTitle(firstMessage.Content),
                "feedback" => "Feedback Session",
                "command" => "Command Request",
                "greeting" => "General Conversation",
                _ => TruncateForTitle(firstMessage.Content)
            };
        }

        return TruncateForTitle(firstMessage.Content);
    }

    private string ExtractQuestionTitle(string message)
    {
        // Try to extract a meaningful question title
        var truncated = TruncateForTitle(message);
        if (truncated.Contains('?'))
            return truncated;
        
        return $"Question: {truncated}";
    }

    private string TruncateForTitle(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return "New Conversation";

        text = text.Trim();
        if (text.Length <= 50)
            return text;

        // Find last space before 50 characters
        int truncateAt = text.LastIndexOf(' ', 47);
        if (truncateAt > 20)
            return text.Substring(0, truncateAt) + "...";

        return text.Substring(0, 47) + "...";
    }

    private double CalculateAverageSentiment(List<Sentiment> sentiments)
    {
        if (!sentiments.Any())
            return 0;

        double sum = sentiments.Sum(s => s switch
        {
            Sentiment.VeryNegative => -2.0,
            Sentiment.Negative => -1.0,
            Sentiment.Neutral => 0.0,
            Sentiment.Positive => 1.0,
            Sentiment.VeryPositive => 2.0,
            _ => 0.0
        });

        return sum / sentiments.Count;
    }

    private string GetSentimentDescription(double avgSentiment)
    {
        return avgSentiment switch
        {
            >= 1.5 => "Very Positive",
            >= 0.5 => "Positive",
            >= -0.5 => "Neutral",
            >= -1.5 => "Negative",
            _ => "Very Negative"
        };
    }

    private List<string> ExtractKeywords(List<string> messages)
    {
        // Simple keyword extraction - remove common words and find most frequent
        var commonWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "the", "is", "at", "which", "on", "a", "an", "and", "or", "but",
            "in", "with", "to", "for", "of", "as", "by", "from", "up", "about",
            "into", "through", "during", "before", "after", "above", "below",
            "can", "could", "would", "should", "will", "do", "does", "did",
            "i", "you", "he", "she", "it", "we", "they", "me", "him", "her",
            "my", "your", "his", "its", "our", "their", "what", "when", "where",
            "how", "why", "who", "which", "this", "that", "these", "those",
            "am", "are", "was", "were", "be", "been", "being", "have", "has",
            "had", "having", "please", "thanks", "thank", "hello", "hi", "hey"
        };

        var words = messages
            .SelectMany(m => m.Split(new[] { ' ', ',', '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries))
            .Where(w => w.Length > 3 && !commonWords.Contains(w))
            .GroupBy(w => w.ToLower())
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .Take(5)
            .ToList();

        return words;
    }
}
