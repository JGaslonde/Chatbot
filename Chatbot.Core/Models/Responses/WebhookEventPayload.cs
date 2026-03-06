namespace Chatbot.Core.Models.Responses;

public record WebhookEventPayload(
    string EventType,
    string EventId,
    DateTime Timestamp,
    int UserId,
    string? ResourceType,
    int? ResourceId,
    Dictionary<string, object> Data
);
