using Chatbot.API.Models.Responses;

namespace Chatbot.API.Services.Core.Interfaces;

/// <summary>
/// System metrics, health monitoring, and performance tracking
/// </summary>
public interface ISystemMetricsService
{
    /// <summary>
    /// Get current system metrics and health status
    /// </summary>
    Task<SystemMetricsDto> GetCurrentMetricsAsync();

    /// <summary>
    /// Get metrics for a specific time period
    /// </summary>
    Task<SystemMetricsDto> GetMetricsForPeriodAsync(
        DateTime startDate,
        DateTime endDate);

    /// <summary>
    /// Get API performance statistics
    /// </summary>
    Task<Dictionary<string, object>> GetApiPerformanceStatsAsync(
        DateTime? startDate = null,
        DateTime? endDate = null);

    /// <summary>
    /// Get database performance metrics
    /// </summary>
    Task<Dictionary<string, object>> GetDatabaseMetricsAsync();

    /// <summary>
    /// Get cache statistics
    /// </summary>
    Task<Dictionary<string, object>> GetCacheStatsAsync();

    /// <summary>
    /// Get memory usage info
    /// </summary>
    Task<Dictionary<string, object>> GetMemoryUsageAsync();

    /// <summary>
    /// Get CPU usage info
    /// </summary>
    Task<Dictionary<string, object>> GetCpuUsageAsync();

    /// <summary>
    /// Get request queue depth
    /// </summary>
    Task<int> GetQueueDepthAsync();

    /// <summary>
    /// Get active session count
    /// </summary>
    Task<int> GetActiveSessionCountAsync();

    /// <summary>
    /// Get API endpoint statistics
    /// </summary>
    Task<List<Dictionary<string, object>>> GetEndpointStatsAsync();

    /// <summary>
    /// Get error statistics
    /// </summary>
    Task<Dictionary<string, object>> GetErrorStatsAsync(
        DateTime? startDate = null,
        DateTime? endDate = null);

    /// <summary>
    /// Perform health check
    /// </summary>
    Task<bool> PerformHealthCheckAsync();

    /// <summary>
    /// Get system health status
    /// </summary>
    Task<string> GetHealthStatusAsync();

    /// <summary>
    /// Get critical alerts
    /// </summary>
    Task<List<string>> GetCriticalAlertsAsync();

    /// <summary>
    /// Clear metrics cache (force refresh)
    /// </summary>
    Task<bool> ClearMetricsCacheAsync();

    /// <summary>
    /// Record custom metric
    /// </summary>
    Task<bool> RecordCustomMetricAsync(
        string metricName,
        double value,
        Dictionary<string, object>? tags = null);
}
