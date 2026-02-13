using Chatbot.Core.Models.Entities;

namespace Chatbot.API.Data.Repositories.Interfaces;

public interface IUserNotificationPreferencesRepository : IRepository<UserNotificationPreferences>
{
    Task<UserNotificationPreferences?> GetByUserIdAsync(int userId);
}
