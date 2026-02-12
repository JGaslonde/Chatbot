using Chatbot.Core.Models;
using System.Net.Http.Json;
using System.Net.Http.Headers;

namespace Chatbot.Web.Services;

public interface IAuthService
{
    Task<(bool Success, string Message, string? Token)> LoginAsync(string username, string password);
    Task<(bool Success, string Message, string? Token)> RegisterAsync(string username, string email, string password);
    Task LogoutAsync();
    string? GetToken();
    bool IsAuthenticated();
}

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly ApiAuthenticationStateProvider _authStateProvider;
    private string? _token;

    public AuthService(HttpClient httpClient, ApiAuthenticationStateProvider authStateProvider)
    {
        _httpClient = httpClient;
        _authStateProvider = authStateProvider;
    }

    public async Task<(bool Success, string Message, string? Token)> LoginAsync(string username, string password)
    {
        try
        {
            var request = new LoginRequest(username, password);
            var response = await _httpClient.PostAsJsonAsync("api/Chat/login", request);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<AuthResponse>>();
                if (result?.Success == true && result.Data != null)
                {
                    _token = result.Data.Token;
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", _token);
                    _authStateProvider.MarkUserAsAuthenticated(result.Data.Username, _token);
                    return (true, result.Message, _token);
                }
            }

            var errorResult = await response.Content.ReadFromJsonAsync<ApiResponse<AuthResponse>>();
            return (false, errorResult?.Message ?? "Login failed", null);
        }
        catch (Exception ex)
        {
            return (false, $"Error: {ex.Message}", null);
        }
    }

    public async Task<(bool Success, string Message, string? Token)> RegisterAsync(string username, string email, string password)
    {
        try
        {
            var request = new CreateUserRequest(username, email, password);
            var response = await _httpClient.PostAsJsonAsync("api/Chat/register", request);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<AuthResponse>>();
                if (result?.Success == true && result.Data != null)
                {
                    _token = result.Data.Token;
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", _token);
                    _authStateProvider.MarkUserAsAuthenticated(result.Data.Username, _token);
                    return (true, result.Message, _token);
                }
            }

            var errorResult = await response.Content.ReadFromJsonAsync<ApiResponse<AuthResponse>>();
            return (false, errorResult?.Message ?? "Registration failed", null);
        }
        catch (Exception ex)
        {
            return (false, $"Error: {ex.Message}", null);
        }
    }

    public Task LogoutAsync()
    {
        _token = null;
        _httpClient.DefaultRequestHeaders.Authorization = null;
        _authStateProvider.MarkUserAsLoggedOut();
        return Task.CompletedTask;
    }

    public string? GetToken() => _token;

    public bool IsAuthenticated() => !string.IsNullOrEmpty(_token);
}
