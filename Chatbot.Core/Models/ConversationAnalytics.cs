using Chatbot.Core.Models.Analytics;

namespace Chatbot.Core.Models;

public class ConversationAnalytics
{
    public int TotalMessages { get; set; }
    public int UserMessageCount { get; set; }
    public int BotMessageCount { get; set; }
    public double AverageSentimentScore { get; set; }
    public Dictionary<string, int> SentimentDistribution { get; set; } = new();
    public Dictionary<string, int> IntentDistribution { get; set; } = new();
    public DateRange DateRange { get; set; } = new();
}
