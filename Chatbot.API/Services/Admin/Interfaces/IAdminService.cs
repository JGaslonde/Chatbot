using Chatbot.Core.Models.Responses;

namespace Chatbot.API.Services.Admin.Interfaces;

public interface IAdminService
{
    Task<SystemStatsDto> GetSystemStatsAsync();
    Task<SystemConfigDto> GetSystemConfigAsync();
    Task<SystemConfigDto> UpdateSystemConfigAsync(SystemConfigUpdateRequest request);
    Task<List<ActiveUserDto>> GetActiveUsersAsync();
    Task ForceUserLogoutAsync(int userId);
}
