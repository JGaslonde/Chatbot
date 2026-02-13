namespace Chatbot.Web.Models;

public record IntentDistribution(
    string Intent,
    int Count,
    double Percentage
);
