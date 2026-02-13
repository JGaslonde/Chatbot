namespace Chatbot.Web.Models;

/// <summary>
/// Analytics data model for web display
/// </summary>
public record ConversationAnalytics(
    int TotalConversations,
    int TotalMessages,
    double AverageSentiment,
    Dictionary<string, int> IntentDistribution,
    Dictionary<string, int> SentimentDistribution,
    DateTime AnalyzedAt
);
