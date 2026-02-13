namespace Chatbot.Core.Models.Entities;

/// <summary>
/// Notification type enumeration.
/// </summary>
public enum NotificationType
{
    Message,
    ConversationUpdate,
    Analytics,
    Mention,
    System
}

/// <summary>
/// Export format enumeration.
/// </summary>
public enum ExportFormat
{
    Json,
    Csv,
    Pdf
}

/// <summary>
/// Audit action enumeration for tracking user actions.
/// </summary>
public enum AuditAction
{
    Create,
    Read,
    Update,
    Delete,
    Export,
    SearchConversation,
    SendMessage,
    UpdatePreferences,
    Login,
    Logout
}
