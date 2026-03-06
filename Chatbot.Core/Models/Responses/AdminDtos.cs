namespace Chatbot.Core.Models.Responses;

public record SystemStatsDto(
    int TotalUsers,
    int ActiveUsers,
    int TotalConversations,
    int TotalMessages,
    double AverageConversationLength,
    double AverageSentiment,
    DateTime ServerStartTime,
    TimeSpan ServerUptime,
    double CpuUsagePercent,
    double MemoryUsageMb,
    string DatabaseStatus
);

public record SystemConfigDto(
    string AppName,
    string AppVersion,
    bool MaintenanceMode,
    int MaxConcurrentUsers,
    int SessionTimeoutMinutes,
    int AuditLogRetentionDays,
    bool EnableAnalytics,
    bool EnableRealtime,
    Dictionary<string, string> CustomSettings
);

public record SystemConfigUpdateRequest(
    bool? MaintenanceMode,
    int? MaxConcurrentUsers,
    int? SessionTimeoutMinutes,
    int? AuditLogRetentionDays,
    bool? EnableAnalytics,
    bool? EnableRealtime,
    Dictionary<string, string>? CustomSettings
);

public record ActiveUserDto(
    int UserId,
    string Username,
    string? FullName,
    DateTime LoginTime,
    DateTime? LastActivityTime,
    string IpAddress,
    string UserAgent
);
