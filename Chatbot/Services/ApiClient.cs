using System.Net.Http.Json;
using System.Text.Json;

namespace Chatbot.Services;

/// <summary>
/// HTTP client for communicating with the Chatbot API
/// </summary>
public class ApiClient
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private string? _authToken;

    public ApiClient(string baseUrl = "http://localhost:5089")
    {
        _baseUrl = baseUrl;
        _httpClient = new HttpClient();
    }

    /// <summary>
    /// Set the authentication token
    /// </summary>
    public void SetAuthToken(string token)
    {
        _authToken = token;
        if (!string.IsNullOrWhiteSpace(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    public async Task<(bool success, string message, string? token)> RegisterAsync(string username, string email, string password)
    {
        try
        {
            var request = new { username, email, password };
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/Chat/register", request);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<JsonElement>(content);
                if (result.TryGetProperty("token", out var tokenProp))
                {
                    return (true, "Registration successful!", tokenProp.GetString());
                }
                return (true, "Registration successful!", null);
            }

            return (false, content, null);
        }
        catch (Exception ex)
        {
            return (false, $"Registration error: {ex.Message}", null);
        }
    }

    /// <summary>
    /// Login user
    /// </summary>
    public async Task<(bool success, string message, string? token)> LoginAsync(string username, string password)
    {
        try
        {
            var request = new { username, password };
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/Chat/login", request);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<JsonElement>(content);
                if (result.TryGetProperty("token", out var tokenProp))
                {
                    var token = tokenProp.GetString();
                    SetAuthToken(token!);
                    return (true, "Login successful!", token);
                }
                return (true, "Login successful!", null);
            }

            return (false, content, null);
        }
        catch (Exception ex)
        {
            return (false, $"Login error: {ex.Message}", null);
        }
    }

    /// <summary>
    /// Send a chat message to the API
    /// </summary>
    public async Task<(bool success, string response)> SendMessageAsync(string conversationId, string message)
    {
        try
        {
            var request = new { message };
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/Chat/{conversationId}/send", request);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<JsonElement>(content);
                if (result.TryGetProperty("response", out var responseProp))
                {
                    return (true, responseProp.GetString() ?? "");
                }
                return (true, content);
            }

            return (false, content);
        }
        catch (Exception ex)
        {
            return (false, $"Error sending message: {ex.Message}");
        }
    }

    /// <summary>
    /// Get conversation history
    /// </summary>
    public async Task<(bool success, List<ConversationMessage> messages)> GetConversationHistoryAsync(string conversationId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/Chat/{conversationId}/history");
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var messages = JsonSerializer.Deserialize<List<ConversationMessage>>(content) ?? new();
                return (true, messages);
            }

            return (false, new());
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error fetching history: {ex.Message}");
            return (false, new());
        }
    }

    /// <summary>
    /// Get list of conversations
    /// </summary>
    public async Task<(bool success, List<ConversationSummary> conversations)> GetConversationsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/Chat/conversations");
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var conversations = JsonSerializer.Deserialize<List<ConversationSummary>>(content) ?? new();
                return (true, conversations);
            }

            return (false, new());
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error fetching conversations: {ex.Message}");
            return (false, new());
        }
    }

    /// <summary>
    /// Get analytics for a conversation
    /// </summary>
    public async Task<string> GetAnalyticsAsync(string conversationId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/Chat/analytics?conversationId={conversationId}");
            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            return $"Error fetching analytics: {ex.Message}";
        }
    }

    /// <summary>
    /// Search conversations
    /// </summary>
    public async Task<(bool success, List<ConversationSummary> results)> SearchConversationsAsync(string query)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(
                $"{_baseUrl}/api/v1/advanced/conversations/search",
                new { query });
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var results = JsonSerializer.Deserialize<List<ConversationSummary>>(content) ?? new();
                return (true, results);
            }

            return (false, new());
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error searching: {ex.Message}");
            return (false, new());
        }
    }

    /// <summary>
    /// Get user preferences
    /// </summary>
    public async Task<string> GetPreferencesAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/Chat/preferences");
            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            return $"Error fetching preferences: {ex.Message}";
        }
    }

    /// <summary>
    /// Update user preferences
    /// </summary>
    public async Task<bool> UpdatePreferencesAsync(Dictionary<string, object> preferences)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"{_baseUrl}/api/Chat/preferences", preferences);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Export conversation
    /// </summary>
    public async Task<string> ExportConversationAsync(string conversationId, string format = "csv")
    {
        try
        {
            var endpoint = format == "json" ? "json" : "csv";
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/Chat/{conversationId}/export/{endpoint}");
            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            return $"Error exporting: {ex.Message}";
        }
    }

    /// <summary>
    /// Get notifications
    /// </summary>
    public async Task<string> GetNotificationsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/Chat/notifications/unread");
            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            return $"Error fetching notifications: {ex.Message}";
        }
    }

    /// <summary>
    /// Get webhooks (Enterprise)
    /// </summary>
    public async Task<string> GetWebhooksAsync(string apiKey)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Remove("X-API-Key");
            _httpClient.DefaultRequestHeaders.Add("X-API-Key", apiKey);
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/v1/enterprise/webhooks");
            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            return $"Error fetching webhooks: {ex.Message}";
        }
    }

    /// <summary>
    /// Get available features/endpoints (health check)
    /// </summary>
    public async Task<bool> HealthCheckAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/Chat/health");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}

/// <summary>
/// Models for API responses
/// </summary>
public class ConversationMessage
{
    public string Sender { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}

public class ConversationSummary
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastMessageAt { get; set; }
    public int MessageCount { get; set; }
}
