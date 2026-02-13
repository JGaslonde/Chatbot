namespace Chatbot.Core.Models.Responses;

public record HealthResponse(
    string Status,
    DateTime Timestamp,
    string Version
);
