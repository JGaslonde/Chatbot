using System.Net.Http.Json;
using Chatbot.Core.Models.Requests;
using Chatbot.Core.Models.Responses;

namespace Chatbot.Web.Services;

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
            var response = await _httpClient.PostAsJsonAsync("api/v1/imports", request);
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
            var response = await _httpClient.GetAsync("api/v1/imports");
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
            var response = await _httpClient.GetAsync($"api/v1/imports/{jobId}");
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
            var response = await _httpClient.PostAsync($"api/v1/imports/{jobId}/upload", content);
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
            var response = await _httpClient.PostAsync($"api/v1/enterprise/imports/{jobId}/process", null);
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
