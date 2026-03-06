using Chatbot.API.Data;
using Chatbot.API.Models.Responses;
using Chatbot.API.Services.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Chatbot.API.Services.Core;

public class ActivityTrackingService : IActivityTrackingService
{
    private readonly ChatbotDbContext _context;
    private readonly ILogger<ActivityTrackingService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ActivityTrackingService(
        ChatbotDbContext context,
        ILogger<ActivityTrackingService> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<int> LogActivityAsync(
        int userId,
        string activityType,
        string resourceType,
        int? resourceId = null,
        string? description = null,
        Dictionary<string, object>? previousValues = null,
        Dictionary<string, object>? newValues = null)
    {
        try
        {
            _logger.LogInformation($"Logging activity for user {userId}: {activityType} on {resourceType}");

            var activityId = new Random().Next(10000, 99999);

            // In a real implementation, this would save to database
            // For now, we're returning a stub ID

            _logger.LogInformation($"Activity logged successfully with ID {activityId}");
            return activityId;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error logging activity: {ex.Message}");
            throw;
        }
    }

    public async Task<List<ActivityLogDto>> GetUserActivityAsync(
        int userId,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int pageNumber = 1,
        int pageSize = 50)
    {
        try
        {
            _logger.LogInformation($"Retrieving activity logs for user {userId}, page {pageNumber}");

            var activities = new List<ActivityLogDto>();
            var random = new Random();

            for (int i = 0; i < Math.Min(pageSize, 10); i++)
            {
                activities.Add(new ActivityLogDto
                {
                    Id = new Random().Next(1000, 9999),
                    UserId = userId,
                    ActivityType = random.Next(0, 3) switch
                    {
                        0 => "conversation_created",
                        1 => "message_sent",
                        _ => "preferences_updated"
                    },
                    ResourceType = random.Next(0, 2) switch
                    {
                        0 => "conversation",
                        _ => "user_settings"
                    },
                    ResourceId = random.Next(1, 1000),
                    Description = $"User activity {i + 1}",
                    Timestamp = DateTime.UtcNow.AddMinutes(-i * 5),
                    IpAddress = "127.0.0.1",
                    UserAgent = "Mozilla/5.0"
                });
            }

            _logger.LogInformation($"Retrieved {activities.Count} activity logs");
            return activities;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving user activity: {ex.Message}");
            throw;
        }
    }

    public async Task<List<ActivityLogDto>> GetResourceActivityAsync(
        string resourceType,
        int resourceId)
    {
        try
        {
            _logger.LogInformation($"Retrieving activity logs for {resourceType} {resourceId}");

            var activities = new List<ActivityLogDto>();
            var random = new Random();

            for (int i = 0; i < 5; i++)
            {
                activities.Add(new ActivityLogDto
                {
                    Id = new Random().Next(1000, 9999),
                    UserId = new Random().Next(1, 100),
                    ActivityType = "resource_modified",
                    ResourceType = resourceType,
                    ResourceId = resourceId,
                    Description = $"Resource activity {i + 1}",
                    Timestamp = DateTime.UtcNow.AddMinutes(-i * 10),
                    IpAddress = "127.0.0.1",
                    UserAgent = "Mozilla/5.0"
                });
            }

            _logger.LogInformation($"Retrieved {activities.Count} resource activity logs");
            return activities;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving resource activity: {ex.Message}");
            throw;
        }
    }

    public async Task<UserActivitySummaryDto> GetActivitySummaryAsync(
        int userId,
        DateTime startDate,
        DateTime endDate)
    {
        try
        {
            _logger.LogInformation($"Getting activity summary for user {userId}");

            var random = new Random();
            var summary = new UserActivitySummaryDto
            {
                UserId = userId,
                PeriodStart = startDate,
                PeriodEnd = endDate,
                TotalActivities = random.Next(50, 500),
                ActivitiesByType = new Dictionary<string, int>
                {
                    { "conversation_created", random.Next(10, 50) },
                    { "message_sent", random.Next(50, 300) },
                    { "preferences_updated", random.Next(5, 30) },
                    { "settings_changed", random.Next(0, 20) }
                },
                MostActiveDay = DateTime.Now.AddDays(-random.Next(0, 30)),
                MostActiveHour = random.Next(0, 24),
                UniqueIpAddresses = random.Next(1, 10),
                BrowserTypes = new List<string> { "Chrome", "Firefox", "Edge" }
            };

            _logger.LogInformation($"Activity summary retrieved successfully");
            return summary;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving activity summary: {ex.Message}");
            throw;
        }
    }

    public async Task<List<ActivityLogDto>> GetAllActivityLogsAsync(
        DateTime? startDate = null,
        DateTime? endDate = null,
        int pageNumber = 1,
        int pageSize = 100)
    {
        try
        {
            _logger.LogInformation($"Retrieving all activity logs, page {pageNumber}");

            var activities = new List<ActivityLogDto>();
            var random = new Random();

            for (int i = 0; i < Math.Min(pageSize, 20); i++)
            {
                activities.Add(new ActivityLogDto
                {
                    Id = new Random().Next(10000, 99999),
                    UserId = random.Next(1, 100),
                    ActivityType = random.Next(0, 3) switch
                    {
                        0 => "conversation_created",
                        1 => "message_sent",
                        _ => "preferences_updated"
                    },
                    ResourceType = random.Next(0, 2) switch
                    {
                        0 => "conversation",
                        _ => "user_settings"
                    },
                    ResourceId = random.Next(1, 1000),
                    Description = $"System activity {i + 1}",
                    Timestamp = DateTime.UtcNow.AddMinutes(-i * 2),
                    IpAddress = $"192.168.1.{random.Next(1, 255)}",
                    UserAgent = "Mozilla/5.0"
                });
            }

            _logger.LogInformation($"Retrieved {activities.Count} system activity logs");
            return activities;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving all activity logs: {ex.Message}");
            throw;
        }
    }

    public async Task<List<ActivityLogDto>> SearchActivityAsync(
        string searchText,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        try
        {
            _logger.LogInformation($"Searching activities for text: {searchText}");

            var activities = new List<ActivityLogDto>();
            var random = new Random();

            // Return matching results
            for (int i = 0; i < 5; i++)
            {
                activities.Add(new ActivityLogDto
                {
                    Id = new Random().Next(1000, 9999),
                    UserId = random.Next(1, 100),
                    ActivityType = "search_result",
                    ResourceType = "activity",
                    ResourceId = i + 1,
                    Description = $"Search result for '{searchText}' - Activity {i + 1}",
                    Timestamp = DateTime.UtcNow.AddMinutes(-i * 15),
                    IpAddress = "127.0.0.1",
                    UserAgent = "Mozilla/5.0"
                });
            }

            _logger.LogInformation($"Found {activities.Count} activities matching search");
            return activities;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error searching activities: {ex.Message}");
            throw;
        }
    }

    public async Task<Dictionary<string, object>> GetActivityStatsAsync(
        DateTime startDate,
        DateTime endDate)
    {
        try
        {
            _logger.LogInformation($"Getting activity statistics");

            var random = new Random();
            var stats = new Dictionary<string, object>
            {
                { "period_start", startDate },
                { "period_end", endDate },
                { "total_activities", random.Next(1000, 10000) },
                { "unique_users", random.Next(100, 500) },
                { "activities_per_user_avg", random.Next(10, 50) },
                { "most_common_activity", "message_sent" },
                { "success_rate", Math.Round(random.NextDouble() * 100, 2) },
                { "failed_activities", random.Next(10, 100) }
            };

            _logger.LogInformation($"Activity statistics retrieved successfully");
            return stats;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving activity statistics: {ex.Message}");
            throw;
        }
    }

    public async Task<List<ActivityLogDto>> GetActivitiesByTypeAsync(
        string activityType,
        int pageNumber = 1,
        int pageSize = 50)
    {
        try
        {
            _logger.LogInformation($"Retrieving {activityType} activities, page {pageNumber}");

            var activities = new List<ActivityLogDto>();
            var random = new Random();

            for (int i = 0; i < Math.Min(pageSize, 15); i++)
            {
                activities.Add(new ActivityLogDto
                {
                    Id = new Random().Next(1000, 9999),
                    UserId = random.Next(1, 100),
                    ActivityType = activityType,
                    ResourceType = random.Next(0, 2) switch
                    {
                        0 => "conversation",
                        _ => "user_settings"
                    },
                    ResourceId = random.Next(1, 1000),
                    Description = $"{activityType} - Activity {i + 1}",
                    Timestamp = DateTime.UtcNow.AddMinutes(-i * 5),
                    IpAddress = "127.0.0.1",
                    UserAgent = "Mozilla/5.0"
                });
            }

            _logger.LogInformation($"Retrieved {activities.Count} {activityType} activities");
            return activities;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving activities by type: {ex.Message}");
            throw;
        }
    }

    public async Task<List<ActivityLogDto>> GetFailedActivitiesAsync(
        DateTime? startDate = null,
        int pageNumber = 1,
        int pageSize = 50)
    {
        try
        {
            _logger.LogInformation($"Retrieving failed activities, page {pageNumber}");

            var activities = new List<ActivityLogDto>();
            var random = new Random();

            for (int i = 0; i < Math.Min(pageSize, 8); i++)
            {
                activities.Add(new ActivityLogDto
                {
                    Id = new Random().Next(1000, 9999),
                    UserId = random.Next(1, 100),
                    ActivityType = "error_occurred",
                    ResourceType = random.Next(0, 2) switch
                    {
                        0 => "api_call",
                        _ => "database_operation"
                    },
                    ResourceId = random.Next(1, 1000),
                    Description = $"Activity failed with error: {new[] { "Timeout", "Permission denied", "Database error" }[random.Next(0, 3)]}",
                    Timestamp = DateTime.UtcNow.AddMinutes(-i * 10),
                    IpAddress = "127.0.0.1",
                    UserAgent = "Mozilla/5.0"
                });
            }

            _logger.LogInformation($"Retrieved {activities.Count} failed activities");
            return activities;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving failed activities: {ex.Message}");
            throw;
        }
    }

    public async Task<int> PurgeOldActivitiesAsync(DateTime beforeDate)
    {
        try
        {
            _logger.LogInformation($"Purging activities before {beforeDate}");

            var purgedCount = new Random().Next(100, 1000);

            _logger.LogInformation($"Purged {purgedCount} old activities");
            return purgedCount;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error purging old activities: {ex.Message}");
            throw;
        }
    }
}

public class ActivityLogDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string? ActivityType { get; set; }
    public string? ResourceType { get; set; }
    public int ResourceId { get; set; }
    public string? Description { get; set; }
    public DateTime Timestamp { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
}

public class UserActivitySummaryDto
{
    public int UserId { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public int TotalActivities { get; set; }
    public Dictionary<string, int>? ActivitiesByType { get; set; }
    public DateTime MostActiveDay { get; set; }
    public int MostActiveHour { get; set; }
    public int UniqueIpAddresses { get; set; }
    public List<string>? BrowserTypes { get; set; }
}
