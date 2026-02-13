namespace Chatbot.Web.Models;

public record SentimentTrend(
    DateTime Date,
    double AverageSentiment,
    int MessageCount
);
