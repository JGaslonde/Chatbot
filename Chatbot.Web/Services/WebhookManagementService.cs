using System.Net.Http.Json;
using Chatbot.Core.Models.Requests;
using Chatbot.Core.Models.Responses;

namespace Chatbot.Web.Services;

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
            var response = await _httpClient.PostAsJsonAsync("api/v1/webhooks", request);
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
            var response = await _httpClient.GetAsync("api/v1/webhooks");
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
            var response = await _httpClient.PutAsJsonAsync($"api/v1/webhooks/{webhookId}", request);
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
            var response = await _httpClient.DeleteAsync($"api/v1/webhooks/{webhookId}");
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
