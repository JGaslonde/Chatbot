namespace Chatbot.Core.Models.Requests;

/// <summary>Request to analyze user segments</summary>
public record UserSegmentAnalysisRequest(
    int? UserId = null,
    string? BehavioralSegment = null,
    string? EngagementLevel = null,
    int? MinChurnRiskThreshold = null,
    bool IncludeChurnPrediction = true
);
