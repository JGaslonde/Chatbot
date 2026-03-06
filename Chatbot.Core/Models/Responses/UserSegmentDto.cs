namespace Chatbot.Core.Models.Responses;

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
