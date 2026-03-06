using Chatbot.Core.Models.Entities;

namespace Chatbot.API.Data.Repositories.Interfaces;

public interface IIpWhitelistRepository : IRepository<IpWhitelist>
{
    Task<IEnumerable<IpWhitelist>> GetUserWhitelistAsync(int userId);
    Task<IpWhitelist?> GetByIpAsync(int userId, string ipAddress);
}
