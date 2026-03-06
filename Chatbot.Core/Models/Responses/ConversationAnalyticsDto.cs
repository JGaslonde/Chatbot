namespace Chatbot.Core.Models.Responses;

/// <summary>Response with conversation analytics data</summary>
public record ConversationAnalyticsDto(
    int Id,
    int ConversationId,
    double AverageSentimentScore,
    int TotalMessages,
    int UserMessageCount,
    int BotMessageCount,
    double AverageResponseTime,
    double AverageMessageLength,
    string? DominantIntent,
    string? DetectedTopics,
    int EngagementScore,
    bool? IsSatisfactory,
    DateTime UpdatedAt
);
