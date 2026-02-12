using Chatbot.Web.Models;
using System.Net.Http.Json;

namespace Chatbot.Web.Services;

public interface IConversationService
{
    Task<(bool Success, string Message, List<ConversationResponse>? Conversations)> GetConversationsAsync();
    Task<(bool Success, string Message, ConversationResponse? Conversation)> GetConversationAsync(int id);
    Task<(bool Success, string Message, byte[]? Data)> ExportConversationAsJsonAsync(int id);
    Task<(bool Success, string Message, byte[]? Data)> ExportConversationAsCsvAsync(int id);
}

public class ConversationService : IConversationService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ConversationService> _logger;

    public ConversationService(HttpClient httpClient, ILogger<ConversationService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<(bool Success, string Message, List<ConversationResponse>? Conversations)> GetConversationsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/Chat/conversations");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<ConversationResponse>>>();
                if (result?.Success == true && result.Data != null)
                {
                    return (true, result.Message, result.Data);
                }
            }

            var errorResult = await response.Content.ReadFromJsonAsync<ApiResponse<List<ConversationResponse>>>();
            _logger.LogWarning("Failed to get conversations: {Message}", errorResult?.Message);
            return (false, errorResult?.Message ?? "Failed to get conversations", null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting conversations");
            return (false, $"Error: {ex.Message}", null);
        }
    }

    public async Task<(bool Success, string Message, ConversationResponse? Conversation)> GetConversationAsync(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/Chat/{id}");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<ConversationResponse>>();
                if (result?.Success == true && result.Data != null)
                {
                    return (true, result.Message, result.Data);
                }
            }

            var errorResult = await response.Content.ReadFromJsonAsync<ApiResponse<ConversationResponse>>();
            _logger.LogWarning("Failed to get conversation: {Message}", errorResult?.Message);
            return (false, errorResult?.Message ?? "Failed to get conversation", null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting conversation");
            return (false, $"Error: {ex.Message}", null);
        }
    }

    public async Task<(bool Success, string Message, byte[]? Data)> ExportConversationAsJsonAsync(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/Chat/{id}/export/json");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsByteArrayAsync();
                return (true, "Export successful", data);
            }

            _logger.LogWarning("Failed to export conversation as JSON");
            return (false, "Failed to export conversation", null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting conversation as JSON");
            return (false, $"Error: {ex.Message}", null);
        }
    }

    public async Task<(bool Success, string Message, byte[]? Data)> ExportConversationAsCsvAsync(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/Chat/{id}/export/csv");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsByteArrayAsync();
                return (true, "Export successful", data);
            }

            _logger.LogWarning("Failed to export conversation as CSV");
            return (false, "Failed to export conversation", null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting conversation as CSV");
            return (false, $"Error: {ex.Message}", null);
        }
    }
}
