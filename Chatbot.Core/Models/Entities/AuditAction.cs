namespace Chatbot.Core.Models.Entities;

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
