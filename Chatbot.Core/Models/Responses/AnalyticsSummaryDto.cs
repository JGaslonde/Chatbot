namespace Chatbot.Core.Models.Responses;

/// <summary>Analytics dashboard summary</summary>
public record AnalyticsSummaryDto(
    DateTime DateRange,
    int TotalConversations,
    double AverageEngagementScore,
    double AverageSentimentScore,
    int TotalMessages,
    double AverageResponseTime,
    List<TopicFrequencyDto> TopTopics,
    List<IntentFrequencyDto> TopIntents,
    Dictionary<string, int> SentimentDistribution,
    List<MLInsightDto> KeyInsights
);
