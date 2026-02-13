using Chatbot.API.Data.Context;
using Chatbot.Core.Models.Entities;
using Chatbot.API.Data.Repositories.Interfaces;

namespace Chatbot.API.Data.Repositories;

public class UserNotificationPreferencesRepository : Repository<UserNotificationPreferences>, IUserNotificationPreferencesRepository
{
    public UserNotificationPreferencesRepository(ChatbotDbContext context) : base(context) { }

    public async Task<UserNotificationPreferences?> GetByUserIdAsync(int userId)
    {
        return await Task.FromResult(
            _context.UserNotificationPreferences
                .FirstOrDefault(p => p.UserId == userId)
        );
    }
}
