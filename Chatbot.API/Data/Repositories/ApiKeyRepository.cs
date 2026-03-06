using Chatbot.API.Data.Repositories.Interfaces;
using Chatbot.Core.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Chatbot.API.Data.Repositories;

public class ApiKeyRepository : Repository<ApiKey>, IApiKeyRepository
{
    public ApiKeyRepository(ChatbotDbContext context) : base(context) { }

    public async Task<IEnumerable<ApiKey>> GetUserApiKeysAsync(int userId)
    {
        return await _context.ApiKeys
            .Where(ak => ak.UserId == userId && ak.IsActive)
            .OrderByDescending(ak => ak.CreatedAt)
            .ToListAsync();
    }

    public async Task<ApiKey?> GetByHashAsync(string keyHash)
    {
        return await _context.ApiKeys
            .FirstOrDefaultAsync(ak => ak.KeyHash == keyHash && ak.IsActive);
    }
}
