using Chatbot.Core.Models.Responses;
using System.Net.Http.Json;

namespace Chatbot.Web.Services;

/// <summary>
/// Service for retrieving advanced analytics data.
/// </summary>
public class AnalyticsDashboardService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AnalyticsDashboardService> _logger;

    public AnalyticsDashboardService(HttpClient httpClient, ILogger<AnalyticsDashboardService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<AdvancedAnalyticsResponse?> GetAdvancedAnalyticsAsync(DateTime fromDate, DateTime toDate)
    {
        try
        {
            var response = await _httpClient.GetAsync(
                $"/api/analytics/advanced?fromDate={fromDate:O}&toDate={toDate:O}");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<AdvancedAnalyticsResponse>>();
                return result?.Data;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving advanced analytics");
        }
        return null;
    }

    public async Task<Dictionary<string, int>?> GetMessageTrendsAsync(DateTime fromDate, DateTime toDate)
    {
        try
        {
            var response = await _httpClient.GetAsync(
                $"/api/analytics/trends?fromDate={fromDate:O}&toDate={toDate:O}");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<Dictionary<string, int>>>();
                return result?.Data;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving message trends");
        }
        return null;
    }

    public async Task<Dictionary<string, int>?> GetIntentDistributionAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/analytics/intents");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<Dictionary<string, int>>>();
                return result?.Data;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving intent distribution");
        }
        return null;
    }
}
