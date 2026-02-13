using Chatbot.Core.Models.Entities;

namespace Chatbot.API.Data.Repositories.Interfaces;

public interface INotificationRepository : IRepository<Notification>
{
    Task<IEnumerable<Notification>> GetUserNotificationsAsync(int userId);
    Task<IEnumerable<Notification>> GetUnreadNotificationsAsync(int userId);
}
