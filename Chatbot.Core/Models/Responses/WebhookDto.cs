namespace Chatbot.Core.Models.Responses;

public record WebhookDto(
    int Id,
    string Url,
    string EventType,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? LastTriggeredAt
);
