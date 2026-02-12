namespace Chatbot.Services;

/// <summary>
/// Sentiment types for message analysis
/// </summary>
public enum Sentiment
{
    VeryNegative,
    Negative,
    Neutral,
    Positive,
    VeryPositive
}

/// <summary>
/// Result of sentiment analysis
/// </summary>
public class SentimentResult
{
    public Sentiment Sentiment { get; set; }
    public double Score { get; set; } // -1.0 to 1.0
}

/// <summary>
/// Simple sentiment analysis service for the console chatbot
/// </summary>
public class SentimentAnalyzer
{
    private readonly Dictionary<string, double> _positiveWords;
    private readonly Dictionary<string, double> _negativeWords;

    public SentimentAnalyzer()
    {
        _positiveWords = new Dictionary<string, double>
        {
            ["excellent"] = 1.0, ["amazing"] = 1.0, ["wonderful"] = 1.0, ["fantastic"] = 1.0,
            ["great"] = 0.8, ["good"] = 0.6, ["nice"] = 0.5, ["love"] = 0.9,
            ["happy"] = 0.7, ["best"] = 0.9, ["awesome"] = 0.9, ["perfect"] = 0.8,
            ["thank"] = 0.5, ["thanks"] = 0.5, ["appreciate"] = 0.6, ["helpful"] = 0.6,
            ["glad"] = 0.6, ["pleased"] = 0.6, ["enjoy"] = 0.7, ["like"] = 0.5
        };

        _negativeWords = new Dictionary<string, double>
        {
            ["terrible"] = -1.0, ["awful"] = -1.0, ["horrible"] = -1.0, ["worst"] = -1.0,
            ["bad"] = -0.6, ["hate"] = -0.9, ["angry"] = -0.7, ["sad"] = -0.6,
            ["upset"] = -0.7, ["annoyed"] = -0.6, ["disappointed"] = -0.7, ["useless"] = -0.8,
            ["poor"] = -0.6, ["fail"] = -0.7, ["broken"] = -0.6, ["problem"] = -0.5,
            ["issue"] = -0.4, ["wrong"] = -0.5, ["difficult"] = -0.4, ["hard"] = -0.3
        };
    }

    /// <summary>
    /// Analyze the sentiment of a message
    /// </summary>
    public SentimentResult Analyze(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return new SentimentResult { Sentiment = Sentiment.Neutral, Score = 0.0 };
        }

        string lowerMessage = message.ToLower();
        string[] words = lowerMessage.Split(new[] { ' ', ',', '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);

        double totalScore = 0.0;
        int matchCount = 0;

        foreach (var word in words)
        {
            if (_positiveWords.TryGetValue(word, out double positiveScore))
            {
                totalScore += positiveScore;
                matchCount++;
            }
            else if (_negativeWords.TryGetValue(word, out double negativeScore))
            {
                totalScore += negativeScore;
                matchCount++;
            }
        }

        // Calculate average score
        double normalizedScore = matchCount > 0 ? totalScore / matchCount : 0.0;

        // Clamp score to -1.0 to 1.0 range
        normalizedScore = Math.Max(-1.0, Math.Min(1.0, normalizedScore));

        // Determine sentiment category
        Sentiment sentiment = normalizedScore switch
        {
            <= -0.6 => Sentiment.VeryNegative,
            <= -0.2 => Sentiment.Negative,
            <= 0.2 => Sentiment.Neutral,
            <= 0.6 => Sentiment.Positive,
            _ => Sentiment.VeryPositive
        };

        return new SentimentResult { Sentiment = sentiment, Score = normalizedScore };
    }
}
