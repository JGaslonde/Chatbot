namespace Chatbot.Core.Models.Responses;

/// <summary>User cohort analysis</summary>
public record UserCohortAnalysisDto(
    string CohortName,
    int UserCount,
    double AverageEngagementScore,
    double AverageSatisfaction,
    double ChurnRate,
    DateTime CreatedAt
);
