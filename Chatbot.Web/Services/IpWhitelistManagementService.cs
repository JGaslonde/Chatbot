using System.Net.Http.Json;
using Chatbot.Core.Models.Requests;
using Chatbot.Core.Models.Responses;

namespace Chatbot.Web.Services;

public class IpWhitelistManagementService : IIpWhitelistManagementService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<IpWhitelistManagementService> _logger;

    public IpWhitelistManagementService(HttpClient httpClient, ILogger<IpWhitelistManagementService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<IpWhitelistDto?> AddIpToWhitelistAsync(IpWhitelistRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/v1/ip-whitelist", request);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<IpWhitelistDto>();
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error adding IP to whitelist: {ex.Message}");
            return null;
        }
    }

    public async Task<List<IpWhitelistDto>> GetWhitelistAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/v1/ip-whitelist");
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<List<IpWhitelistDto>>();
            return result ?? new List<IpWhitelistDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving whitelist: {ex.Message}");
            return new List<IpWhitelistDto>();
        }
    }

    public async Task<bool> RemoveIpFromWhitelistAsync(int whitelistId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/v1/ip-whitelist/{whitelistId}");
            response.EnsureSuccessStatusCode();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error removing IP from whitelist: {ex.Message}");
            return false;
        }
    }
}
