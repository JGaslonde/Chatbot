using System.Net.Http.Json;
using Chatbot.Core.Models.Requests;
using Chatbot.Core.Models.Responses;

namespace Chatbot.Web.Services;

public interface IWebhookManagementService
{
    Task<WebhookDto?> CreateWebhookAsync(WebhookRequest request);
    Task<List<WebhookDto>> GetWebhooksAsync();
    Task<bool> UpdateWebhookAsync(int webhookId, WebhookRequest request);
    Task<bool> DeleteWebhookAsync(int webhookId);
}

public class WebhookManagementService : IWebhookManagementService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<WebhookManagementService> _logger;

    public WebhookManagementService(HttpClient httpClient, ILogger<WebhookManagementService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<WebhookDto?> CreateWebhookAsync(WebhookRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/phase2/webhooks", request);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<WebhookDto>();
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error creating webhook: {ex.Message}");
            return null;
        }
    }

    public async Task<List<WebhookDto>> GetWebhooksAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/phase2/webhooks");
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<List<WebhookDto>>();
            return result ?? new List<WebhookDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving webhooks: {ex.Message}");
            return new List<WebhookDto>();
        }
    }

    public async Task<bool> UpdateWebhookAsync(int webhookId, WebhookRequest request)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"api/phase2/webhooks/{webhookId}", request);
            response.EnsureSuccessStatusCode();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating webhook: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DeleteWebhookAsync(int webhookId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/phase2/webhooks/{webhookId}");
            response.EnsureSuccessStatusCode();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting webhook: {ex.Message}");
            return false;
        }
    }
}

public interface IApiKeyManagementService
{
    Task<ApiKeyCreateResponse?> GenerateApiKeyAsync(ApiKeyRequest request);
    Task<List<ApiKeyDto>> GetApiKeysAsync();
    Task<bool> RevokeApiKeyAsync(int apiKeyId);
}

public class ApiKeyManagementService : IApiKeyManagementService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApiKeyManagementService> _logger;

    public ApiKeyManagementService(HttpClient httpClient, ILogger<ApiKeyManagementService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<ApiKeyCreateResponse?> GenerateApiKeyAsync(ApiKeyRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/phase2/api-keys", request);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<ApiKeyCreateResponse>();
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error generating API key: {ex.Message}");
            return null;
        }
    }

    public async Task<List<ApiKeyDto>> GetApiKeysAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/phase2/api-keys");
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<List<ApiKeyDto>>();
            return result ?? new List<ApiKeyDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving API keys: {ex.Message}");
            return new List<ApiKeyDto>();
        }
    }

    public async Task<bool> RevokeApiKeyAsync(int apiKeyId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/phase2/api-keys/{apiKeyId}");
            response.EnsureSuccessStatusCode();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error revoking API key: {ex.Message}");
            return false;
        }
    }
}

public interface ITwoFactorManagementService
{
    Task<TwoFactorSetupResponse?> Setup2FAAsync();
    Task<bool> Enable2FAAsync(string secret, string verificationCode);
    Task<List<string>> Disable2FAAsync();
}

public class TwoFactorManagementService : ITwoFactorManagementService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<TwoFactorManagementService> _logger;

    public TwoFactorManagementService(HttpClient httpClient, ILogger<TwoFactorManagementService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<TwoFactorSetupResponse?> Setup2FAAsync()
    {
        try
        {
            var response = await _httpClient.PostAsync("api/phase2/2fa/setup", null);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<TwoFactorSetupResponse>();
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error setting up 2FA: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> Enable2FAAsync(string secret, string verificationCode)
    {
        try
        {
            var request = new { Secret = secret, VerificationCode = verificationCode };
            var response = await _httpClient.PostAsJsonAsync("api/phase2/2fa/enable", request);
            response.EnsureSuccessStatusCode();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error enabling 2FA: {ex.Message}");
            return false;
        }
    }

    public async Task<List<string>> Disable2FAAsync()
    {
        try
        {
            var response = await _httpClient.PostAsync("api/phase2/2fa/disable", null);
            response.EnsureSuccessStatusCode();

            var resultContent = await response.Content.ReadAsStringAsync();
            // Parse the response to get backup codes
            return new List<string>();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error disabling 2FA: {ex.Message}");
            return new List<string>();
        }
    }
}

public interface IIpWhitelistManagementService
{
    Task<IpWhitelistDto?> AddIpToWhitelistAsync(IpWhitelistRequest request);
    Task<List<IpWhitelistDto>> GetWhitelistAsync();
    Task<bool> RemoveIpFromWhitelistAsync(int whitelistId);
}

public class IpWhitelistManagementService : IIpWhitelistManagementService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<IpWhitelistManagementService> _logger;

    public IpWhitelistManagementService(HttpClient httpClient, ILogger<IpWhitelistManagementService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<IpWhitelistDto?> AddIpToWhitelistAsync(IpWhitelistRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/phase2/ip-whitelist", request);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<IpWhitelistDto>();
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error adding IP to whitelist: {ex.Message}");
            return null;
        }
    }

    public async Task<List<IpWhitelistDto>> GetWhitelistAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/phase2/ip-whitelist");
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<List<IpWhitelistDto>>();
            return result ?? new List<IpWhitelistDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving whitelist: {ex.Message}");
            return new List<IpWhitelistDto>();
        }
    }

    public async Task<bool> RemoveIpFromWhitelistAsync(int whitelistId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/phase2/ip-whitelist/{whitelistId}");
            response.EnsureSuccessStatusCode();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error removing IP from whitelist: {ex.Message}");
            return false;
        }
    }
}
