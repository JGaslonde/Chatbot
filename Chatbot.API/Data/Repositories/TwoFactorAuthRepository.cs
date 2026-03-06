using Chatbot.API.Data.Repositories.Interfaces;
using Chatbot.Core.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Chatbot.API.Data.Repositories;

public class TwoFactorAuthRepository : Repository<TwoFactorAuth>, ITwoFactorAuthRepository
{
    public TwoFactorAuthRepository(ChatbotDbContext context) : base(context) { }

    public async Task<TwoFactorAuth?> GetByUserIdAsync(int userId)
    {
        return await _context.TwoFactorAuths
            .FirstOrDefaultAsync(tfa => tfa.UserId == userId);
    }
}
