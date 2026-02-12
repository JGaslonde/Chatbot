using Chatbot.API.Models.Entities;

namespace Chatbot.API.Services;

public interface ISentimentAnalysisService
{
    Task<(Sentiment Sentiment, double Score)> AnalyzeSentimentAsync(string text);
}

public class SimpleSentimentAnalysisService : ISentimentAnalysisService
{
    private readonly Dictionary<string, Sentiment> _sentimentWords = new()
    {
        // Positive words
        { "good", Sentiment.Positive },
        { "great", Sentiment.VeryPositive },
        { "excellent", Sentiment.VeryPositive },
        { "amazing", Sentiment.VeryPositive },
        { "wonderful", Sentiment.VeryPositive },
        { "happy", Sentiment.Positive },
        { "love", Sentiment.VeryPositive },
        { "like", Sentiment.Positive },
        { "fantastic", Sentiment.VeryPositive },
        { "awesome", Sentiment.VeryPositive },
        { "perfect", Sentiment.VeryPositive },
        { "thanks", Sentiment.Positive },
        
        // Negative words
        { "bad", Sentiment.Negative },
        { "terrible", Sentiment.VeryNegative },
        { "awful", Sentiment.VeryNegative },
        { "horrible", Sentiment.VeryNegative },
        { "hate", Sentiment.VeryNegative },
        { "dislike", Sentiment.Negative },
        { "sad", Sentiment.Negative },
        { "angry", Sentiment.Negative },
        { "disappointed", Sentiment.Negative },
        { "frustrated", Sentiment.Negative },
        { "useless", Sentiment.VeryNegative },
        { "waste", Sentiment.Negative }
    };

    public async Task<(Sentiment Sentiment, double Score)> AnalyzeSentimentAsync(string text)
    {
        var lower = text.ToLower();
        var words = lower.Split(new[] { ' ', ',', '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
        
        double score = 0;
        int matchCount = 0;

        foreach (var word in words)
        {
                if (_sentimentWords.TryGetValue(word, out var sentimentValue))
                {
                    matchCount++;
                    score += sentimentValue switch
                    Sentiment.Negative => -0.5,
                    Sentiment.Neutral => 0.0,
                    Sentiment.Positive => 0.5,
                    Sentiment.VeryPositive => 1.0,
                    _ => 0.0
                };
            }
        }

        if (matchCount == 0)
            return (Sentiment.Neutral, 0.0);

        var averageScore = score / matchCount;
        var sentiment = averageScore switch
        {
            < -0.7 => Sentiment.VeryNegative,
            < -0.3 => Sentiment.Negative,
            < 0.3 => Sentiment.Neutral,
            < 0.7 => Sentiment.Positive,
            _ => Sentiment.VeryPositive
        };

        return (sentiment, Math.Clamp(averageScore, -1.0, 1.0));
    }
}
