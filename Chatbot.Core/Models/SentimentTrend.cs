namespace Chatbot.Core.Models;

public class SentimentTrend
{
    public DateTime Date { get; set; }
    public double AvergeSentiment { get; set; }
    public int MessageCount { get; set; }
}
