using Chatbot.Core.Models.Requests;
using Chatbot.Core.Models.Responses;
using System.Net.Http.Json;

namespace Chatbot.Web.Services;

/// <summary>
/// Service for batch operations on conversations.
/// </summary>
public class BatchOperationService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<BatchOperationService> _logger;

    public BatchOperationService(HttpClient httpClient, ILogger<BatchOperationService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<BatchOperationResponse?> DeleteConversationsAsync(List<int> conversationIds)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/batch/delete", conversationIds);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<BatchOperationResponse>>();
                return result?.Data;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting conversations");
        }
        return null;
    }

    public async Task<BatchOperationResponse?> ArchiveConversationsAsync(List<int> conversationIds)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/batch/archive", conversationIds);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<BatchOperationResponse>>();
                return result?.Data;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error archiving conversations");
        }
        return null;
    }
}
