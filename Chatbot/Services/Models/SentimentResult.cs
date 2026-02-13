using Chatbot.Services.Models;

namespace Chatbot.Services.Models;

/// <summary>
/// Result of sentiment analysis
/// </summary>
public class SentimentResult
{
    public Sentiment Sentiment { get; set; }
    public double Score { get; set; } // -1.0 to 1.0
}
