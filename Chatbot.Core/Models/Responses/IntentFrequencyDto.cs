namespace Chatbot.Core.Models.Responses;

/// <summary>Intent frequency in analytics</summary>
public record IntentFrequencyDto(
    string Intent,
    int Frequency,
    double Percentage,
    double AverageSentimentScore
);
