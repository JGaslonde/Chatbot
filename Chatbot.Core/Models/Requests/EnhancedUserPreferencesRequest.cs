namespace Chatbot.Core.Models.Requests;

public record EnhancedUserPreferencesRequest(
    string? Theme = null,
    string? Language = null,
    bool? EnableEmailNotifications = null,
    bool? EnablePushNotifications = null,
    string? TimeZone = null,
    Dictionary<string, object>? CustomSettings = null
);
