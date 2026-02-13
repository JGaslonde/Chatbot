using Chatbot.Core.Models.Entities;
using Chatbot.Core.Models.Responses;
using Chatbot.API.Data.Repositories.Interfaces;
using Chatbot.API.Services.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Chatbot.API.Services.Core;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IUserNotificationPreferencesRepository _preferencesRepository;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        INotificationRepository notificationRepository,
        IUserNotificationPreferencesRepository preferencesRepository,
        ILogger<NotificationService> logger)
    {
        _notificationRepository = notificationRepository;
        _preferencesRepository = preferencesRepository;
        _logger = logger;
    }

    public async Task<List<NotificationDto>> GetUnreadNotificationsAsync(int userId)
    {
        try
        {
            var notifications = (await _notificationRepository.GetUnreadNotificationsAsync(userId)).ToList();

            return notifications.Select(n => new NotificationDto(
                n.Id,
                n.UserId,
                n.Type,
                n.Message,
                n.RelatedConversationId,
                n.IsRead,
                n.CreatedAt
            )).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving unread notifications for user {UserId}", userId);
            throw;
        }
    }

    public async Task MarkNotificationAsReadAsync(int notificationId)
    {
        try
        {
            var notification = await _notificationRepository.GetByIdAsync(notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;
                await _notificationRepository.UpdateAsync(notification);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking notification {NotificationId} as read", notificationId);
            throw;
        }
    }

    public async Task MarkAllNotificationsAsReadAsync(int userId)
    {
        try
        {
            var notifications = (await _notificationRepository.GetUserNotificationsAsync(userId)).ToList();

            foreach (var notification in notifications.Where(n => !n.IsRead))
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;
                await _notificationRepository.UpdateAsync(notification);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking all notifications as read for user {UserId}", userId);
            throw;
        }
    }

    public async Task<NotificationDto> CreateNotificationAsync(
        int userId,
        string type,
        string message,
        int? relatedConversationId = null)
    {
        try
        {
            var preferences = await _preferencesRepository.GetByUserIdAsync(userId);

            // Check if notifications are enabled
            if (preferences != null && !preferences.EnableNotifications)
            {
                _logger.LogDebug("Notifications disabled for user {UserId}", userId);
                throw new InvalidOperationException("Notifications are disabled for this user");
            }

            var notification = new Notification
            {
                UserId = userId,
                Type = type,
                Message = message,
                RelatedConversationId = relatedConversationId,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _notificationRepository.AddAsync(notification);

            return new NotificationDto(
                created.Id,
                created.UserId,
                created.Type,
                created.Message,
                created.RelatedConversationId,
                created.IsRead,
                created.CreatedAt
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating notification for user {UserId}", userId);
            throw;
        }
    }

    public async Task DeleteNotificationAsync(int notificationId)
    {
        try
        {
            await _notificationRepository.DeleteAsync(notificationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting notification {NotificationId}", notificationId);
            throw;
        }
    }
}
