using Chatbot.API.Data.Context;
using Chatbot.Core.Models.Entities;
using Chatbot.API.Data.Repositories.Interfaces;

namespace Chatbot.API.Data.Repositories;

public class NotificationRepository : Repository<Notification>, INotificationRepository
{
    public NotificationRepository(ChatbotDbContext context) : base(context) { }

    public async Task<IEnumerable<Notification>> GetUserNotificationsAsync(int userId)
    {
        return await Task.FromResult(
            _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToList()
        );
    }

    public async Task<IEnumerable<Notification>> GetUnreadNotificationsAsync(int userId)
    {
        return await Task.FromResult(
            _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .OrderByDescending(n => n.CreatedAt)
                .ToList()
        );
    }
}
