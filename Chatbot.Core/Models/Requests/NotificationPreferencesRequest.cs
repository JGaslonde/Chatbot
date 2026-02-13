namespace Chatbot.Core.Models.Requests;

/// <summary>
/// Request to update user notification preferences.
/// </summary>
public record NotificationPreferencesRequest(
    bool EnableNotifications = true,
    bool NotifyOnNewMessage = true,
    bool NotifyOnConversationUpdate = true,
    bool NotifyOnAnalyticsUpdate = false,
    string NotificationFrequency = "immediate" // "immediate", "daily", "weekly"
);
