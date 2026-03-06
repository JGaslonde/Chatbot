namespace Chatbot.Core.Models.Entities;

/// <summary>
/// Webhook event types.
/// </summary>
public enum WebhookEventType
{
    ConversationStarted,
    ConversationEnded,
    MessageReceived,
    MessageAnalyzed,
    UserRegistered,
    UserDeleted,
    ConversationArchived,
    ConversationDeleted
}
