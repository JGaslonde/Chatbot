using Chatbot.API.Models.Responses;

namespace Chatbot.API.Services.Core.Interfaces;

/// <summary>
/// User activity tracking and audit logging
/// </summary>
public interface IActivityTrackingService
{
    /// <summary>
    /// Log a user activity
    /// </summary>
    Task<int> LogActivityAsync(
        int userId,
        string activityType,
        string resourceType,
        int? resourceId = null,
        string? description = null,
        Dictionary<string, object>? previousValues = null,
        Dictionary<string, object>? newValues = null);

    /// <summary>
    /// Get activity log for user
    /// </summary>
    Task<List<ActivityLogDto>> GetUserActivityAsync(
        int userId,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int pageNumber = 1,
        int pageSize = 50);

    /// <summary>
    /// Get activity log for specific resource
    /// </summary>
    Task<List<ActivityLogDto>> GetResourceActivityAsync(
        string resourceType,
        int resourceId);

    /// <summary>
    /// Get activity summary for user
    /// </summary>
    Task<UserActivitySummaryDto> GetActivitySummaryAsync(
        int userId,
        DateTime startDate,
        DateTime endDate);

    /// <summary>
    /// Get all activity logs (admin only)
    /// </summary>
    Task<List<ActivityLogDto>> GetAllActivityLogsAsync(
        DateTime? startDate = null,
        DateTime? endDate = null,
        int pageNumber = 1,
        int pageSize = 100);

    /// <summary>
    /// Search activity logs
    /// </summary>
    Task<List<ActivityLogDto>> SearchActivityAsync(
        string searchText,
        DateTime? startDate = null,
        DateTime? endDate = null);

    /// <summary>
    /// Get activity statistics
    /// </summary>
    Task<Dictionary<string, object>> GetActivityStatsAsync(
        DateTime startDate,
        DateTime endDate);

    /// <summary>
    /// Filter activities by type
    /// </summary>
    Task<List<ActivityLogDto>> GetActivitiesByTypeAsync(
        string activityType,
        int pageNumber = 1,
        int pageSize = 50);

    /// <summary>
    /// Get failed activities (errors)
    /// </summary>
    Task<List<ActivityLogDto>> GetFailedActivitiesAsync(
        DateTime? startDate = null,
        int pageNumber = 1,
        int pageSize = 50);

    /// <summary>
    /// Delete activity log (purge old entries)
    /// </summary>
    Task<int> PurgeOldActivitiesAsync(DateTime beforeDate);
}
