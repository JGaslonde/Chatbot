using Chatbot.Core.Models.Requests;
using Chatbot.Core.Models.Responses;
using System.Net.Http.Json;

namespace Chatbot.Web.Services;

/// <summary>
/// Service for advanced search and filtering of conversations.
/// </summary>
public class AdvancedSearchService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AdvancedSearchService> _logger;

    public AdvancedSearchService(HttpClient httpClient, ILogger<AdvancedSearchService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<SearchResultsResponse?> SearchConversationsAsync(SearchConversationsRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/search/conversations", request);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<SearchResultsResponse>>();
                return result?.Data;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching conversations");
        }
        return null;
    }

    public async Task<SearchResultsResponse?> SearchMessagesAsync(string query, int page = 1, int pageSize = 20)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/search/messages?query={Uri.EscapeDataString(query)}&page={page}&pageSize={pageSize}");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<SearchResultsResponse>>();
                return result?.Data;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching messages");
        }
        return null;
    }
}
