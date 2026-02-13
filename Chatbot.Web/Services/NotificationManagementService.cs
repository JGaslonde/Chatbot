using Chatbot.Core.Models.Responses;
using System.Net.Http.Json;

namespace Chatbot.Web.Services;

/// <summary>
/// Service for notification management.
/// </summary>
public class NotificationManagementService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<NotificationManagementService> _logger;

    public NotificationManagementService(HttpClient httpClient, ILogger<NotificationManagementService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<NotificationDto>?> GetUnreadNotificationsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/notifications/unread");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<NotificationDto>>>();
                return result?.Data;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving unread notifications");
        }
        return null;
    }

    public async Task<bool> MarkNotificationAsReadAsync(int notificationId)
    {
        try
        {
            var response = await _httpClient.PutAsync($"/api/notifications/{notificationId}/read", null);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking notification as read");
        }
        return false;
    }

    public async Task<bool> MarkAllNotificationsAsReadAsync()
    {
        try
        {
            var response = await _httpClient.PutAsync("/api/notifications/mark-all-read", null);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking all notifications as read");
        }
        return false;
    }

    public async Task<bool> DeleteNotificationAsync(int notificationId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/api/notifications/{notificationId}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting notification");
        }
        return false;
    }
}
