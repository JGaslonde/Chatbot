using Chatbot.Core.Models.Requests;
using Chatbot.Core.Models.Responses;

namespace Chatbot.API.Services.Core.Interfaces;

/// <summary>
/// Service for IP whitelisting.
/// </summary>
public interface IIpWhitelistService
{
    Task<IpWhitelistDto> AddIpToWhitelistAsync(int userId, IpWhitelistRequest request);
    Task<List<IpWhitelistDto>> GetUserWhitelistAsync(int userId);
    Task<bool> IsIpWhitelistedAsync(int userId, string ipAddress);
    Task<bool> RemoveIpFromWhitelistAsync(int userId, int whitelistId);
}
