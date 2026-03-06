using System.Net.Http.Json;
using Chatbot.Core.Models.Requests;
using Chatbot.Core.Models.Responses;

namespace Chatbot.Web.Services;

public class ReportingManagementService : IReportingManagementService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ReportingManagementService> _logger;

    public ReportingManagementService(HttpClient httpClient, ILogger<ReportingManagementService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<ScheduledReportDto?> CreateScheduledReportAsync(ScheduledReportRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/v1/reports", request);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<ScheduledReportDto>();
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error creating scheduled report: {ex.Message}");
            return null;
        }
    }

    public async Task<List<ScheduledReportDto>> GetReportsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/v1/reports");
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<List<ScheduledReportDto>>();
            return result ?? new List<ScheduledReportDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving reports: {ex.Message}");
            return new List<ScheduledReportDto>();
        }
    }

    public async Task<bool> UpdateScheduledReportAsync(int reportId, ScheduledReportRequest request)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"api/v1/reports/{reportId}", request);
            response.EnsureSuccessStatusCode();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating scheduled report: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DeleteScheduledReportAsync(int reportId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/v1/reports/{reportId}");
            response.EnsureSuccessStatusCode();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting scheduled report: {ex.Message}");
            return false;
        }
    }

    public async Task<byte[]?> GenerateReportAsync(int reportId, string format = "pdf")
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/v1/reports/{reportId}/generate?format={format}");
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsByteArrayAsync();
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error generating report: {ex.Message}");
            return null;
        }
    }
}
