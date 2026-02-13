namespace Chatbot.Core.Models.Requests;

public record WebhookRequest(
    string Url,
    string? Secret,
    string EventType,
    bool IsActive = true,
    int MaxRetries = 3
);

public record ApiKeyRequest(
    string Name,
    string? Description,
    int? ExpirationDays = null
);

public record TwoFactorSetupRequest(
    string? Secret = null,
    string? VerificationCode = null
);

public record IpWhitelistRequest(
    string IpAddress,
    string? Description,
    int? ExpirationDays = null
);

public record ScheduledReportRequest(
    string Name,
    string? Description,
    string ReportType,
    string Frequency,
    string? RecipientEmail
);

public record StartImportRequest(
    string ImportType,
    int FileSize,
    string? FileName = null
);

public record EnhancedUserPreferencesRequest(
    string? Theme = null,
    string? Language = null,
    bool? EnableEmailNotifications = null,
    bool? EnablePushNotifications = null,
    string? TimeZone = null,
    Dictionary<string, object>? CustomSettings = null
);
