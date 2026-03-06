using Chatbot.Core.Models.Requests;
using Chatbot.Core.Models.Responses;

namespace Chatbot.Web.Services;

public interface IIpWhitelistManagementService
{
    Task<IpWhitelistDto?> AddIpToWhitelistAsync(IpWhitelistRequest request);
    Task<List<IpWhitelistDto>> GetWhitelistAsync();
    Task<bool> RemoveIpFromWhitelistAsync(int whitelistId);
}
