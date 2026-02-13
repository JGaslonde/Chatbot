namespace Chatbot.Core.Models.Responses;

/// <summary>
/// Notification for real-time updates.
/// </summary>
public record NotificationDto(
    int Id,
    int UserId,
    string Type, // "message", "conversation_update", "analytics", "mention"
    string Message,
    int? RelatedConversationId,
    bool IsRead,
    DateTime CreatedAt
);
