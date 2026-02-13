namespace Chatbot.Core.Models.Responses;

public record MessageDto(
    int Id,
    string Content,
    string Sender,
    DateTime SentAt,
    string Sentiment,
    string? Intent,
    double SentimentScore
);
