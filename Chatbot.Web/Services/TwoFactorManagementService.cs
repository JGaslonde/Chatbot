using System.Net.Http.Json;
using Chatbot.Core.Models.Responses;

namespace Chatbot.Web.Services;

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
            var response = await _httpClient.PostAsync("api/v1/twofactor/setup", null);
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
            var response = await _httpClient.PostAsJsonAsync("api/v1/twofactor/enable", request);
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
            var response = await _httpClient.PostAsync("api/v1/twofactor/disable", null);
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
