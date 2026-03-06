using System.Net.Http.Json;
using Chatbot.Core.Models.Requests;
using Chatbot.Core.Models.Responses;

namespace Chatbot.Web.Services;

public class UserPreferencesManagementService : IUserPreferencesManagementService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<UserPreferencesManagementService> _logger;

    public UserPreferencesManagementService(HttpClient httpClient, ILogger<UserPreferencesManagementService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<EnhancedUserPreferencesDto?> GetPreferencesAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/v1/preferences");
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<EnhancedUserPreferencesDto>();
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving preferences: {ex.Message}");
            return null;
        }
    }

    public async Task<EnhancedUserPreferencesDto?> UpdatePreferencesAsync(EnhancedUserPreferencesRequest request)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync("api/v1/preferences", request);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<EnhancedUserPreferencesDto>();
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating preferences: {ex.Message}");
            return null;
        }
    }
}
