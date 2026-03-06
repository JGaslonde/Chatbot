namespace Chatbot.Core.Models.Requests;

/// <summary>Request to generate analytics report</summary>
public record AnalyticsReportRequest(
    string ReportType, // conversation, user, engagement, sentiment, topic, intent
    DateTime DateFrom,
    DateTime DateTo,
    string? Format = "json", // json, csv, pdf
    bool IncludeComparisons = false,
    bool IncludePredictions = false
);
