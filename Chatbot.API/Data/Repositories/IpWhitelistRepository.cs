using Chatbot.API.Data.Repositories.Interfaces;
using Chatbot.Core.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Chatbot.API.Data.Repositories;

public class IpWhitelistRepository : Repository<IpWhitelist>, IIpWhitelistRepository
{
    public IpWhitelistRepository(ChatbotDbContext context) : base(context) { }

    public async Task<IEnumerable<IpWhitelist>> GetUserWhitelistAsync(int userId)
    {
        return await _context.IpWhitelists
            .Where(iw => iw.UserId == userId && iw.IsActive)
            .OrderByDescending(iw => iw.CreatedAt)
            .ToListAsync();
    }

    public async Task<IpWhitelist?> GetByIpAsync(int userId, string ipAddress)
    {
        return await _context.IpWhitelists
            .FirstOrDefaultAsync(iw => iw.UserId == userId && iw.IpAddress == ipAddress && iw.IsActive);
    }
}
