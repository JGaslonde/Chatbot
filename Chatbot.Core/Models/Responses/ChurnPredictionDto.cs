namespace Chatbot.Core.Models.Responses;

/// <summary>Churn prediction result</summary>
public record ChurnPredictionDto(
    int UserId,
    double ChurnRiskScore,
    DateTime? PredictedChurnDate,
    List<string> RiskFactors,
    List<string> RetentionRecommendations
);
