namespace Chatbot.Core.Models.Responses;

public record ChatMessageResponse(
    string Message,
    DateTime Timestamp,
    string Intent,
    double IntentConfidence,
    string Sentiment,
    double SentimentScore,
    int ConversationId
);
