namespace Chatbot.Core.Models.Requests;

/// <summary>Request to create or analyze conversation analytics</summary>
public record AnalyticsRequest(
    int ConversationId,
    string? DateRangeStart = null,
    string? DateRangeEnd = null,
    bool IncludeTrendAnalysis = false
);

/// <summary>Request to generate ML insights</summary>
public record MLInsightRequest(
    string InsightType,
    int? MinimumSampleSize = 10,
    double MinimumConfidence = 0.7,
    string? DataFilter = null
);

/// <summary>Request to create a workflow automation definition</summary>
public record WorkflowDefinitionRequest(
    string Name,
    string? Description,
    string TriggerCondition,
    List<WorkflowStepRequest> Steps,
    bool IsActive = true
);

/// <summary>Individual step in a workflow</summary>
public record WorkflowStepRequest(
    int StepNumber,
    string Action,
    Dictionary<string, object>? Parameters = null,
    string? Condition = null
);

/// <summary>Request to execute a workflow immediately</summary>
public record WorkflowExecutionRequest(
    int WorkflowDefinitionId,
    int ConversationId,
    Dictionary<string, object>? Overrides = null
);

/// <summary>Request to search conversations</summary>
public record SearchRequest(
    string Query,
    string? SearchType = "all", // all, content, topic, intent, keyword
    DateTime? DateFrom = null,
    DateTime? DateTo = null,
    int PageNumber = 1,
    int PageSize = 20,
    string? SortBy = "relevance" // relevance, date, sentiment
);

/// <summary>Request to analyze user segments</summary>
public record UserSegmentAnalysisRequest(
    int? UserId = null,
    string? BehavioralSegment = null,
    string? EngagementLevel = null,
    int? MinChurnRiskThreshold = null,
    bool IncludeChurnPrediction = true
);

/// <summary>Request to generate analytics report</summary>
public record AnalyticsReportRequest(
    string ReportType, // conversation, user, engagement, sentiment, topic, intent
    DateTime DateFrom,
    DateTime DateTo,
    string? Format = "json", // json, csv, pdf
    bool IncludeComparisons = false,
    bool IncludePredictions = false
);
