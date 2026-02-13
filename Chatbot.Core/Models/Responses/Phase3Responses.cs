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

/// <summary>Response with workflow definition</summary>
public record WorkflowDefinitionDto(
    int Id,
    string Name,
    string? Description,
    string TriggerCondition,
    List<WorkflowStepDto> WorkflowSteps,
    bool IsActive,
    int ExecutionCount,
    DateTime CreatedAt,
    DateTime? LastExecutedAt
);

/// <summary>Individual workflow step data</summary>
public record WorkflowStepDto(
    int StepNumber,
    string Action,
    Dictionary<string, object>? Parameters,
    string? Condition
);

/// <summary>Response with workflow execution history</summary>
public record WorkflowExecutionDto(
    int Id,
    int WorkflowDefinitionId,
    int ConversationId,
    string Status,
    string? ErrorMessage,
    DateTime StartedAt,
    DateTime? CompletedAt
);

/// <summary>Response with user segment analysis</summary>
public record UserSegmentDto(
    int Id,
    int UserId,
    string EngagementLevel,
    string BehavioralSegment,
    double ChurnRiskScore,
    double AverageDailyConversations,
    double AverageSatisfaction,
    string? PrimaryUseCase,
    DateTime? LastInteractionDate,
    DateTime? PredictedChurnDate,
    DateTime UpdatedAt
);

/// <summary>Response with search results</summary>
public record SearchResultDto(
    int Id,
    int ConversationId,
    string Content,
    string? Keywords,
    string? Topics,
    string? Intents,
    double RelevanceScore,
    DateTime CreatedAt
);

/// <summary>Paginated search results</summary>
public record SearchResultsPageDto(
    List<SearchResultDto> Results,
    int PageNumber,
    int PageSize,
    int TotalResults,
    int TotalPages
);

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

/// <summary>Topic frequency in analytics</summary>
public record TopicFrequencyDto(
    string Topic,
    int Frequency,
    double Percentage
);

/// <summary>Intent frequency in analytics</summary>
public record IntentFrequencyDto(
    string Intent,
    int Frequency,
    double Percentage,
    double AverageSentimentScore
);

/// <summary>User cohort analysis</summary>
public record UserCohortAnalysisDto(
    string CohortName,
    int UserCount,
    double AverageEngagementScore,
    double AverageSatisfaction,
    double ChurnRate,
    DateTime CreatedAt
);

/// <summary>Churn prediction result</summary>
public record ChurnPredictionDto(
    int UserId,
    double ChurnRiskScore,
    DateTime? PredictedChurnDate,
    List<string> RiskFactors,
    List<string> RetentionRecommendations
);
