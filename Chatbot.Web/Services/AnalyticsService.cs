using Chatbot.Web.Models;
using System.Net.Http.Json;

namespace Chatbot.Web.Services;

/// <summary>
/// Analytics data models for display
/// </summary>
public record ConversationAnalytics(
    int TotalConversations,
    int TotalMessages,
    double AverageSentiment,
    Dictionary<string, int> IntentDistribution,
    Dictionary<string, int> SentimentDistribution,
    DateTime AnalyzedAt
);

public record SentimentTrend(
    DateTime Date,
    double AverageSentiment,
    int MessageCount
);

public record IntentDistribution(
    string Intent,
    int Count,
    double Percentage
);

public interface IAnalyticsService
{
    Task<(bool Success, string Message, ConversationAnalytics? Analytics)> GetAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<(bool Success, string Message, List<SentimentTrend>? Trends)> GetSentimentTrendsAsync(int days = 7);
    Task<(bool Success, string Message, List<IntentDistribution>? Distribution)> GetIntentDistributionAsync(int days = 30);
}

public class AnalyticsService : IAnalyticsService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AnalyticsService> _logger;

    public AnalyticsService(HttpClient httpClient, ILogger<AnalyticsService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<(bool Success, string Message, ConversationAnalytics? Analytics)> GetAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var query = new List<string>();
            if (startDate.HasValue)
                query.Add($"startDate={startDate:O}");
            if (endDate.HasValue)
                query.Add($"endDate={endDate:O}");

            var queryString = query.Count > 0 ? "?" + string.Join("&", query) : "";
            var response = await _httpClient.GetAsync($"api/Chat/analytics{queryString}");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<ConversationAnalytics>>();
                if (result?.Success == true && result.Data != null)
                {
                    return (true, result.Message, result.Data);
                }
            }

            var errorResult = await response.Content.ReadFromJsonAsync<ApiResponse<ConversationAnalytics>>();
            _logger.LogWarning("Failed to get analytics: {Message}", errorResult?.Message);
            return (false, errorResult?.Message ?? "Failed to get analytics", null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting analytics");
            return (false, $"Error: {ex.Message}", null);
        }
    }

    public async Task<(bool Success, string Message, List<SentimentTrend>? Trends)> GetSentimentTrendsAsync(int days = 7)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/Chat/analytics/sentiment-trends?days={days}");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<SentimentTrend>>>();
                if (result?.Success == true && result.Data != null)
                {
                    return (true, result.Message, result.Data);
                }
            }

            var errorResult = await response.Content.ReadFromJsonAsync<ApiResponse<List<SentimentTrend>>>();
            _logger.LogWarning("Failed to get sentiment trends: {Message}", errorResult?.Message);
            return (false, errorResult?.Message ?? "Failed to get sentiment trends", null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting sentiment trends");
            return (false, $"Error: {ex.Message}", null);
        }
    }

    public async Task<(bool Success, string Message, List<IntentDistribution>? Distribution)> GetIntentDistributionAsync(int days = 30)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/Chat/analytics/intent-distribution?days={days}");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<IntentDistribution>>>();
                if (result?.Success == true && result.Data != null)
                {
                    return (true, result.Message, result.Data);
                }
            }

            var errorResult = await response.Content.ReadFromJsonAsync<ApiResponse<List<IntentDistribution>>>();
            _logger.LogWarning("Failed to get intent distribution: {Message}", errorResult?.Message);
            return (false, errorResult?.Message ?? "Failed to get intent distribution", null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting intent distribution");
            return (false, $"Error: {ex.Message}", null);
        }
    }
}
