namespace Chatbot.Core.Models.Responses;

public record WebhookDto(
    int Id,
    string Url,
    string EventType,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? LastTriggeredAt
);

public record ApiKeyDto(
    int Id,
    string Name,
    string? Description,
    DateTime CreatedAt,
    DateTime? ExpiresAt,
    DateTime? LastUsedAt,
    bool IsActive
);

public record ApiKeyCreateResponse(
    int Id,
    string Key, // Only shown once
    string Name,
    DateTime CreatedAt
);

public record TwoFactorSetupResponse(
    string Secret,
    string QrCodeUrl,
    List<string> BackupCodes
);

public record IpWhitelistDto(
    int Id,
    string IpAddress,
    string? Description,
    DateTime CreatedAt,
    DateTime? ExpiresAt,
    bool IsActive
);

public record ScheduledReportDto(
    int Id,
    string Name,
    string? Description,
    string ReportType,
    string Frequency,
    string? RecipientEmail,
    bool IsActive,
    DateTime? LastGeneratedAt,
    DateTime? NextGeneratedAt
);

public record ImportJobDto(
    int Id,
    string FileName,
    string ImportType,
    string Status,
    int TotalRecords,
    int ProcessedRecords,
    int FailedRecords,
    DateTime CreatedAt,
    DateTime? CompletedAt
);

public record EnhancedUserPreferencesDto(
    int UserId,
    string? Theme,
    string? Language,
    bool EnableEmailNotifications,
    bool EnablePushNotifications,
    string? TimeZone,
    Dictionary<string, object>? CustomSettings
);

public record WebhookEventPayload(
    string EventType,
    string EventId,
    DateTime Timestamp,
    int UserId,
    string? ResourceType,
    int? ResourceId,
    Dictionary<string, object> Data
);
