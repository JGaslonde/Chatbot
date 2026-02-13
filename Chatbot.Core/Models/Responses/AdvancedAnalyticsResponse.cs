namespace Chatbot.Core.Models.Responses;

/// <summary>
/// Response for advanced analytics dashboard with aggregated metrics.
/// </summary>
public record AdvancedAnalyticsResponse(
    int TotalConversations,
    int TotalMessages,
    double AverageSentimentScore,
    Dictionary<string, int> IntentDistribution,
    Dictionary<string, int> DailyMessageCount,
    double AverageConversationDuration,
    DateTime AnalyticsFrom,
    DateTime AnalyticsTo
);
