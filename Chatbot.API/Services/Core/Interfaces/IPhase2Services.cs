using Chatbot.Core.Models.Requests;
using Chatbot.Core.Models.Responses;

namespace Chatbot.API.Services.Core.Interfaces;

/// <summary>
/// Service for managing webhooks and external integrations.
/// </summary>
public interface IWebhookService
{
    Task<WebhookDto?> CreateWebhookAsync(int userId, WebhookRequest request);
    Task<List<WebhookDto>> GetUserWebhooksAsync(int userId);
    Task<bool> UpdateWebhookAsync(int userId, int webhookId, WebhookRequest request);
    Task<bool> DeleteWebhookAsync(int userId, int webhookId);
    Task TriggerWebhookAsync(int userId, string eventType, string resourceType, int? resourceId, Dictionary<string, object> data);
    Task ProcessFailedDeliveriesAsync();
}

/// <summary>
/// Service for API key management.
/// </summary>
public interface IApiKeyService
{
    Task<ApiKeyCreateResponse> GenerateApiKeyAsync(int userId, ApiKeyRequest request);
    Task<List<ApiKeyDto>> GetUserApiKeysAsync(int userId);
    Task<bool> ValidateApiKeyAsync(string key);
    Task<int?> GetUserIdByApiKeyAsync(string key);
    Task<bool> RevokeApiKeyAsync(int userId, int apiKeyId);
}

/// <summary>
/// Service for two-factor authentication.
/// </summary>
public interface ITwoFactorService
{
    Task<TwoFactorSetupResponse> GenerateTwoFactorSetupAsync(int userId);
    Task<bool> VerifyTwoFactorAsync(int userId, string code);
    Task<bool> EnableTwoFactorAsync(int userId, string secret, string verificationCode);
    Task<List<string>> DisableTwoFactorAsync(int userId);
    Task<bool> VerifyTwoFactorLoginAsync(int userId, string code);
}

/// <summary>
/// Service for IP whitelisting.
/// </summary>
public interface IIpWhitelistService
{
    Task<IpWhitelistDto> AddIpToWhitelistAsync(int userId, IpWhitelistRequest request);
    Task<List<IpWhitelistDto>> GetUserWhitelistAsync(int userId);
    Task<bool> IsIpWhitelistedAsync(int userId, string ipAddress);
    Task<bool> RemoveIpFromWhitelistAsync(int userId, int whitelistId);
}

/// <summary>
/// Service for advanced reporting.
/// </summary>
public interface IReportingService
{
    Task<ScheduledReportDto> CreateScheduledReportAsync(int userId, ScheduledReportRequest request);
    Task<List<ScheduledReportDto>> GetUserReportsAsync(int userId);
    Task<byte[]> GenerateReportAsync(int userId, int reportId, string format = "pdf");
    Task<bool> UpdateScheduledReportAsync(int userId, int reportId, ScheduledReportRequest request);
    Task<bool> DeleteScheduledReportAsync(int userId, int reportId);
    Task ProcessScheduledReportsAsync();
}

/// <summary>
/// Service for bulk import operations.
/// </summary>
public interface IImportService
{
    Task<ImportJobDto> StartImportAsync(int userId, StartImportRequest request);
    Task<bool> UploadChunkAsync(int jobId, byte[] chunk);
    Task ProcessImportAsync(int jobId);
    Task<List<ImportJobDto>> GetUserImportJobsAsync(int userId);
    Task<ImportJobDto?> GetImportJobStatusAsync(int userId, int jobId);
}

/// <summary>
/// Service for advanced caching.
/// </summary>
public interface ICacheService
{
    Task<T?> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
    Task RemoveAsync(string key);
    Task RemoveByPatternAsync(string pattern);
    Task ClearAllAsync();
}

/// <summary>
/// Service for enhanced user preferences.
/// </summary>
public interface IUserPreferencesEnhancedService
{
    Task<EnhancedUserPreferencesDto> GetEnhancedPreferencesAsync(int userId);
    Task<EnhancedUserPreferencesDto> UpdateEnhancedPreferencesAsync(int userId, EnhancedUserPreferencesRequest request);
}
