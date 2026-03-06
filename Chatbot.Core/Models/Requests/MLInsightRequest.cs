namespace Chatbot.Core.Models.Requests;

/// <summary>Request to generate ML insights</summary>
public record MLInsightRequest(
    string InsightType,
    int? MinimumSampleSize = 10,
    double MinimumConfidence = 0.7,
    string? DataFilter = null
);
