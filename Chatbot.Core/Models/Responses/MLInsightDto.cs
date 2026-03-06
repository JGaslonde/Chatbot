namespace Chatbot.Core.Models.Responses;

/// <summary>Response with ML insights</summary>
public record MLInsightDto(
    int Id,
    string InsightType,
    string InsightValue,
    double Confidence,
    int SampleSize,
    string? DetailedData,
    string? RecommendedAction,
    DateTime GeneratedAt,
    bool IsReviewed
);
