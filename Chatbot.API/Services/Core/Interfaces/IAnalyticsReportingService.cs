using Chatbot.API.Models.Requests;
using Chatbot.API.Models.Responses;

namespace Chatbot.API.Services.Core.Interfaces;

/// <summary>
/// Advanced analytics reporting and custom report generation
/// </summary>
public interface IAnalyticsReportingService
{
    /// <summary>
    /// Generate a custom analytics report
    /// </summary>
    Task<AnalyticsReportDto> GenerateReportAsync(
        AnalyticsReportRequest request,
        int userId);

    /// <summary>
    /// Generate quick summary statistics
    /// </summary>
    Task<Dictionary<string, object>> GetSummaryStatsAsync(
        int userId,
        DateTime startDate,
        DateTime endDate);

    /// <summary>
    /// Get time series metrics data
    /// </summary>
    Task<List<MetricDataPoint>> GetMetricsTimeSeriesAsync(
        int userId,
        DateTime startDate,
        DateTime endDate,
        string groupBy = "day");

    /// <summary>
    /// Get conversation trends
    /// </summary>
    Task<Dictionary<string, object>> GetConversationTrendsAsync(
        int userId,
        DateTime startDate,
        DateTime endDate);

    /// <summary>
    /// Get sentiment analysis trends
    /// </summary>
    Task<Dictionary<string, object>> GetSentimentTrendsAsync(
        int userId,
        DateTime startDate,
        DateTime endDate);

    /// <summary>
    /// Get user engagement metrics
    /// </summary>
    Task<Dictionary<string, object>> GetEngagementMetricsAsync(
        int userId,
        DateTime startDate,
        DateTime endDate);

    /// <summary>
    /// Compare metrics between two periods
    /// </summary>
    Task<Dictionary<string, object>> ComparePeriodsAsync(
        int userId,
        DateTime period1Start,
        DateTime period1End,
        DateTime period2Start,
        DateTime period2End);

    /// <summary>
    /// Get top conversations by metric
    /// </summary>
    Task<List<ConversationSummaryDto>> GetTopConversationsAsync(
        int userId,
        string metricType,
        int count = 10);

    /// <summary>
    /// Export report to file (CSV, PDF, JSON)
    /// </summary>
    Task<byte[]> ExportReportAsync(
        AnalyticsReportDto report,
        string format);

    /// <summary>
    /// Schedule a recurring report
    /// </summary>
    Task<int> ScheduleReportAsync(
        int userId,
        string title,
        string schedule,
        string reportType);

    /// <summary>
    /// Get previously generated reports
    /// </summary>
    Task<List<AnalyticsReportDto>> GetReportHistoryAsync(
        int userId,
        int pageNumber = 1,
        int pageSize = 20);
}
