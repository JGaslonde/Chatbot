using System.Net.Http.Json;
using Chatbot.Core.Models.Requests;
using Chatbot.Core.Models.Responses;

namespace Chatbot.Web.Services;

public interface IReportingManagementService
{
    Task<ScheduledReportDto?> CreateScheduledReportAsync(ScheduledReportRequest request);
    Task<List<ScheduledReportDto>> GetReportsAsync();
    Task<bool> UpdateScheduledReportAsync(int reportId, ScheduledReportRequest request);
    Task<bool> DeleteScheduledReportAsync(int reportId);
    Task<byte[]?> GenerateReportAsync(int reportId, string format = "pdf");
}

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
            var response = await _httpClient.PostAsJsonAsync("api/phase2/reports", request);
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
            var response = await _httpClient.GetAsync("api/phase2/reports");
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
            var response = await _httpClient.PutAsJsonAsync($"api/phase2/reports/{reportId}", request);
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
            var response = await _httpClient.DeleteAsync($"api/phase2/reports/{reportId}");
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
            var response = await _httpClient.GetAsync($"api/phase2/reports/{reportId}/generate?format={format}");
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

public interface IImportManagementService
{
    Task<ImportJobDto?> StartImportAsync(StartImportRequest request);
    Task<List<ImportJobDto>> GetImportJobsAsync();
    Task<ImportJobDto?> GetImportJobStatusAsync(int jobId);
    Task<bool> UploadChunkAsync(int jobId, byte[] chunk);
    Task<bool> ProcessImportAsync(int jobId);
}

public class ImportManagementService : IImportManagementService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ImportManagementService> _logger;

    public ImportManagementService(HttpClient httpClient, ILogger<ImportManagementService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<ImportJobDto?> StartImportAsync(StartImportRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/phase2/imports", request);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<ImportJobDto>();
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error starting import: {ex.Message}");
            return null;
        }
    }

    public async Task<List<ImportJobDto>> GetImportJobsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/phase2/imports");
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<List<ImportJobDto>>();
            return result ?? new List<ImportJobDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving import jobs: {ex.Message}");
            return new List<ImportJobDto>();
        }
    }

    public async Task<ImportJobDto?> GetImportJobStatusAsync(int jobId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/phase2/imports/{jobId}");
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<ImportJobDto>();
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving import job status: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> UploadChunkAsync(int jobId, byte[] chunk)
    {
        try
        {
            var content = new ByteArrayContent(chunk);
            var response = await _httpClient.PostAsync($"api/phase2/imports/{jobId}/upload", content);
            response.EnsureSuccessStatusCode();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error uploading chunk: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> ProcessImportAsync(int jobId)
    {
        try
        {
            var response = await _httpClient.PostAsync($"api/phase2/imports/{jobId}/process", null);
            response.EnsureSuccessStatusCode();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error processing import: {ex.Message}");
            return false;
        }
    }
}

public interface IUserPreferencesManagementService
{
    Task<EnhancedUserPreferencesDto?> GetPreferencesAsync();
    Task<EnhancedUserPreferencesDto?> UpdatePreferencesAsync(EnhancedUserPreferencesRequest request);
}

public class UserPreferencesManagementService : IUserPreferencesManagementService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<UserPreferencesManagementService> _logger;

    public UserPreferencesManagementService(HttpClient httpClient, ILogger<UserPreferencesManagementService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<EnhancedUserPreferencesDto?> GetPreferencesAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/phase2/preferences");
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<EnhancedUserPreferencesDto>();
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving preferences: {ex.Message}");
            return null;
        }
    }

    public async Task<EnhancedUserPreferencesDto?> UpdatePreferencesAsync(EnhancedUserPreferencesRequest request)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync("api/phase2/preferences", request);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<EnhancedUserPreferencesDto>();
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating preferences: {ex.Message}");
            return null;
        }
    }
}
