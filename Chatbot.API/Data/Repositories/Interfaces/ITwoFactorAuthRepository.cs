using Chatbot.Core.Models.Entities;

namespace Chatbot.API.Data.Repositories.Interfaces;

public interface ITwoFactorAuthRepository : IRepository<TwoFactorAuth>
{
    Task<TwoFactorAuth?> GetByUserIdAsync(int userId);
}
