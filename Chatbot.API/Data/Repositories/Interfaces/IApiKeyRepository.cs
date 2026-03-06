using Chatbot.Core.Models.Entities;

namespace Chatbot.API.Data.Repositories.Interfaces;

public interface IApiKeyRepository : IRepository<ApiKey>
{
    Task<IEnumerable<ApiKey>> GetUserApiKeysAsync(int userId);
    Task<ApiKey?> GetByHashAsync(string keyHash);
}
