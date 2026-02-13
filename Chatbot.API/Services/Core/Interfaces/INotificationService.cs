using Chatbot.Core.Models.Responses;

namespace Chatbot.API.Services.Core.Interfaces;

/// <summary>
/// Service for managing user notifications and notification preferences.
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Gets unread notifications for a user.
    /// </summary>
    Task<List<NotificationDto>> GetUnreadNotificationsAsync(int userId);

    /// <summary>
    /// Marks a notification as read.
    /// </summary>
    Task MarkNotificationAsReadAsync(int notificationId);

    /// <summary>
    /// Marks all notifications as read for a user.
    /// </summary>
    Task MarkAllNotificationsAsReadAsync(int userId);

    /// <summary>
    /// Creates a notification for a user.
    /// </summary>
    Task<NotificationDto> CreateNotificationAsync(
        int userId,
        string type,
        string message,
        int? relatedConversationId = null);

    /// <summary>
    /// Deletes a notification.
    /// </summary>
    Task DeleteNotificationAsync(int notificationId);
}
