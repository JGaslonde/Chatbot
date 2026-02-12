using Chatbot.Core.Models;
using System.Net.Http.Json;

namespace Chatbot.Web.Services;

/// <summary>
/// User preferences model
/// </summary>
public class UserPreferences
{
    public int UserId { get; set; }
    public string? Theme { get; set; } = "light";
    public string? Language { get; set; } = "en";
    public bool NotificationsEnabled { get; set; } = true;
    public bool DarkMode { get; set; } = false;
    public int MessagePageSize { get; set; } = 20;
    public DateTime? UpdatedAt { get; set; }
}

public interface IPreferencesService
{
    Task<(bool Success, string Message, UserPreferences? Preferences)> GetPreferencesAsync();
    Task<(bool Success, string Message, UserPreferences? Preferences)> UpdatePreferencesAsync(UserPreferences preferences);
}

public class PreferencesService : IPreferencesService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PreferencesService> _logger;

    public PreferencesService(HttpClient httpClient, ILogger<PreferencesService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<(bool Success, string Message, UserPreferences? Preferences)> GetPreferencesAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/Chat/preferences");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<UserPreferences>>();
                if (result?.Success == true && result.Data != null)
                {
                    return (true, result.Message, result.Data);
                }
            }

            var errorResult = await response.Content.ReadFromJsonAsync<ApiResponse<UserPreferences>>();
            _logger.LogWarning("Failed to get preferences: {Message}", errorResult?.Message);
            return (false, errorResult?.Message ?? "Failed to get preferences", null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting preferences");
            return (false, $"Error: {ex.Message}", null);
        }
    }

    public async Task<(bool Success, string Message, UserPreferences? Preferences)> UpdatePreferencesAsync(UserPreferences preferences)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync("api/Chat/preferences", preferences);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<UserPreferences>>();
                if (result?.Success == true && result.Data != null)
                {
                    return (true, result.Message, result.Data);
                }
            }

            var errorResult = await response.Content.ReadFromJsonAsync<ApiResponse<UserPreferences>>();
            _logger.LogWarning("Failed to update preferences: {Message}", errorResult?.Message);
            return (false, errorResult?.Message ?? "Failed to update preferences", null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating preferences");
            return (false, $"Error: {ex.Message}", null);
        }
    }
}
