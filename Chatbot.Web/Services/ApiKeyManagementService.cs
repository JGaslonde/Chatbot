using System.Net.Http.Json;
using Chatbot.Core.Models.Requests;
using Chatbot.Core.Models.Responses;

namespace Chatbot.Web.Services;

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
            var response = await _httpClient.PostAsJsonAsync("api/v1/enterprise/api-keys", request);
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
            var response = await _httpClient.GetAsync("api/v1/enterprise/api-keys");
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
            var response = await _httpClient.DeleteAsync($"api/v1/enterprise/api-keys/{apiKeyId}");
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
