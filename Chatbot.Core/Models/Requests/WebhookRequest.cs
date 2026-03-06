namespace Chatbot.Core.Models.Requests;

public record WebhookRequest(
    string Url,
    string? Secret,
    string EventType,
    bool IsActive = true,
    int MaxRetries = 3
);
