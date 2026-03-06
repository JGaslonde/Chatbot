namespace Chatbot.Core.Models.Responses;

/// <summary>Topic frequency in analytics</summary>
public record TopicFrequencyDto(
    string Topic,
    int Frequency,
    double Percentage
);
