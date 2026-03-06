namespace Chatbot.Core.Models.Responses;

public record EnhancedUserPreferencesDto(
    int UserId,
    string? Theme,
    string? Language,
    bool EnableEmailNotifications,
    bool EnablePushNotifications,
    string? TimeZone,
    Dictionary<string, object>? CustomSettings
);
