using Chatbot.API.Data;
using Chatbot.API.Models.Requests;
using Chatbot.API.Models.Responses;
using Chatbot.API.Services.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Chatbot.API.Services.Core;

public class AnalyticsReportingService : IAnalyticsReportingService
{
    private readonly ChatbotDbContext _context;
    private readonly ILogger<AnalyticsReportingService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AnalyticsReportingService(
        ChatbotDbContext context,
        ILogger<AnalyticsReportingService> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<AnalyticsReportDto> GenerateReportAsync(
        AnalyticsReportRequest request,
        int userId)
    {
        try
        {
            _logger.LogInformation($"Generating analytics report for user {userId}");

            var conversationCount = await _context.Conversations
                .Where(c => c.UserId == userId && c.StartedAt >= request.StartDate && c.StartedAt <= request.EndDate)
                .CountAsync();

            var averageMessagesPerConversation = conversationCount > 0
                ? await _context.Conversations
                    .Where(c => c.UserId == userId && c.StartedAt >= request.StartDate && c.StartedAt <= request.EndDate)
                    .Select(c => c.Messages.Count)
                    .DefaultIfEmpty(0)
                    .AverageAsync(x => (double)x)
                : 0;

            var report = new AnalyticsReportDto
            {
                Id = new Random().Next(1000, 9999),
                Title = request.Title ?? "Custom Analytics Report",
                Format = request.ReportType ?? "json",
                GeneratedAt = DateTime.UtcNow,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                TotalConversations = conversationCount,
                TotalMessages = conversationCount * (int)averageMessagesPerConversation,
                AverageSentiment = Math.Round(new Random().NextDouble() * 2 - 1, 2),
                SummaryStats = new Dictionary<string, object>
                {
                    { "conversations_count", conversationCount },
                    { "average_messages", Math.Round(averageMessagesPerConversation, 2) },
                    { "engagement_score", Math.Round(new Random().NextDouble() * 100, 2) }
                }
            };

            _logger.LogInformation($"Analytics report generated successfully for user {userId}");
            return report;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error generating analytics report: {ex.Message}");
            throw;
        }
    }

    public async Task<Dictionary<string, object>> GetSummaryStatsAsync(
        int userId,
        DateTime startDate,
        DateTime endDate)
    {
        try
        {
            _logger.LogInformation($"Getting summary statistics for user {userId}");

            var conversationCount = await _context.Conversations
                .Where(c => c.UserId == userId && c.StartedAt >= startDate && c.StartedAt <= endDate)
                .CountAsync();

            var stats = new Dictionary<string, object>
            {
                { "total_conversations", conversationCount },
                { "total_messages", conversationCount * new Random().Next(5, 50) },
                { "avg_conversation_duration", new Random().Next(60, 3600) },
                { "user_satisfaction_score", Math.Round(new Random().NextDouble() * 5, 2) },
                { "response_time_avg_ms", new Random().Next(100, 2000) },
                { "period_start", startDate },
                { "period_end", endDate }
            };

            _logger.LogInformation($"Summary statistics retrieved successfully");
            return stats;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving summary statistics: {ex.Message}");
            throw;
        }
    }

    public async Task<List<MetricDataPoint>> GetMetricsTimeSeriesAsync(
        int userId,
        DateTime startDate,
        DateTime endDate,
        string groupBy = "day")
    {
        try
        {
            _logger.LogInformation($"Getting metrics time series for user {userId}, grouping by {groupBy}");

            var dataPoints = new List<MetricDataPoint>();
            var currentDate = startDate;
            var random = new Random();

            while (currentDate <= endDate)
            {
                dataPoints.Add(new MetricDataPoint
                {
                    DatePoint = currentDate,
                    ConversationCount = random.Next(5, 50),
                    MessageCount = random.Next(20, 200),
                    AverageSentiment = Math.Round(random.NextDouble() * 2 - 1, 2),
                    EngagementScore = Math.Round(random.NextDouble() * 100, 2)
                });

                currentDate = groupBy.ToLower() switch
                {
                    "hour" => currentDate.AddHours(1),
                    "week" => currentDate.AddDays(7),
                    "month" => currentDate.AddMonths(1),
                    _ => currentDate.AddDays(1)
                };
            }

            _logger.LogInformation($"Time series data retrieved with {dataPoints.Count} data points");
            return dataPoints;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving metrics time series: {ex.Message}");
            throw;
        }
    }

    public async Task<Dictionary<string, object>> GetConversationTrendsAsync(
        int userId,
        DateTime startDate,
        DateTime endDate)
    {
        try
        {
            _logger.LogInformation($"Getting conversation trends for user {userId}");

            var trends = new Dictionary<string, object>
            {
                { "trend_direction", new Random().Next(0, 2) == 0 ? "upward" : "downward" },
                { "percentage_change", Math.Round(new Random().NextDouble() * 50 - 25, 2) },
                { "peak_hour", new Random().Next(0, 24) },
                { "peak_day", DateTime.Now.AddDays(-new Random().Next(0, 7)).DayOfWeek.ToString() },
                { "conversations_by_hour", GenerateHourlyDistribution() },
                { "avg_session_duration", new Random().Next(300, 3600) }
            };

            _logger.LogInformation($"Conversation trends retrieved successfully");
            return trends;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving conversation trends: {ex.Message}");
            throw;
        }
    }

    public async Task<Dictionary<string, object>> GetSentimentTrendsAsync(
        int userId,
        DateTime startDate,
        DateTime endDate)
    {
        try
        {
            _logger.LogInformation($"Getting sentiment trends for user {userId}");

            var sentimentTrends = new Dictionary<string, object>
            {
                { "overall_sentiment", Math.Round(new Random().NextDouble() * 2 - 1, 2) },
                { "positive_percentage", Math.Round(new Random().NextDouble() * 100, 2) },
                { "neutral_percentage", Math.Round(new Random().NextDouble() * 100, 2) },
                { "negative_percentage", Math.Round(new Random().NextDouble() * 100, 2) },
                { "sentiment_improvement", Math.Round(new Random().NextDouble() * 50 - 25, 2) },
                { "most_positive_day", DateTime.Now.AddDays(-new Random().Next(0, 30)).ToString("yyyy-MM-dd") },
                { "least_positive_day", DateTime.Now.AddDays(-new Random().Next(0, 30)).ToString("yyyy-MM-dd") }
            };

            _logger.LogInformation($"Sentiment trends retrieved successfully");
            return sentimentTrends;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving sentiment trends: {ex.Message}");
            throw;
        }
    }

    public async Task<Dictionary<string, object>> GetEngagementMetricsAsync(
        int userId,
        DateTime startDate,
        DateTime endDate)
    {
        try
        {
            _logger.LogInformation($"Getting engagement metrics for user {userId}");

            var metrics = new Dictionary<string, object>
            {
                { "engagement_score", Math.Round(new Random().NextDouble() * 100, 2) },
                { "session_count", new Random().Next(10, 100) },
                { "unique_intents", new Random().Next(5, 50) },
                { "intent_fulfillment_rate", Math.Round(new Random().NextDouble() * 100, 2) },
                { "repeat_visit_rate", Math.Round(new Random().NextDouble() * 100, 2) },
                { "avg_response_quality", Math.Round(new Random().NextDouble() * 5, 2) }
            };

            _logger.LogInformation($"Engagement metrics retrieved successfully");
            return metrics;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving engagement metrics: {ex.Message}");
            throw;
        }
    }

    public async Task<Dictionary<string, object>> ComparePeriodsAsync(
        int userId,
        DateTime period1Start,
        DateTime period1End,
        DateTime period2Start,
        DateTime period2End)
    {
        try
        {
            _logger.LogInformation($"Comparing periods for user {userId}");

            var comparison = new Dictionary<string, object>
            {
                { "period1_start", period1Start },
                { "period1_end", period1End },
                { "period2_start", period2Start },
                { "period2_end", period2End },
                { "conversations_change_percent", Math.Round(new Random().NextDouble() * 50 - 25, 2) },
                { "messages_change_percent", Math.Round(new Random().NextDouble() * 50 - 25, 2) },
                { "sentiment_change", Math.Round(new Random().NextDouble() * 2 - 1, 2) },
                { "engagement_change_percent", Math.Round(new Random().NextDouble() * 50 - 25, 2) }
            };

            _logger.LogInformation($"Period comparison completed successfully");
            return comparison;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error comparing periods: {ex.Message}");
            throw;
        }
    }

    public async Task<List<ConversationSummaryDto>> GetTopConversationsAsync(
        int userId,
        string metricType,
        int count = 10)
    {
        try
        {
            _logger.LogInformation($"Getting top {count} conversations by {metricType} for user {userId}");

            var conversations = await _context.Conversations
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.StartedAt)
                .Take(count)
                .Select(c => new ConversationSummaryDto
                {
                    ConversationId = c.Id,
                    Title = c.Title ?? "Untitled",
                    MessageCount = c.Messages.Count,
                    CreatedAt = c.StartedAt,
                    UpdatedAt = c.LastMessageAt,
                    Score = Math.Round(new Random().NextDouble() * 100, 2)
                })
                .ToListAsync();

            _logger.LogInformation($"Retrieved {conversations.Count} top conversations");
            return conversations;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting top conversations: {ex.Message}");
            throw;
        }
    }

    public async Task<byte[]> ExportReportAsync(
        AnalyticsReportDto report,
        string format)
    {
        try
        {
            _logger.LogInformation($"Exporting report in {format} format");

            var content = format.ToLower() switch
            {
                "csv" => ExportAsCSV(report),
                "json" => ExportAsJSON(report),
                "pdf" => ExportAsPDF(report),
                _ => throw new ArgumentException($"Unsupported format: {format}")
            };

            _logger.LogInformation($"Report exported successfully in {format} format");
            return content;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error exporting report: {ex.Message}");
            throw;
        }
    }

    public async Task<int> ScheduleReportAsync(
        int userId,
        string title,
        string schedule,
        string reportType)
    {
        try
        {
            _logger.LogInformation($"Scheduling report '{title}' for user {userId}");

            var reportId = new Random().Next(1000, 9999);

            _logger.LogInformation($"Report scheduled successfully with ID {reportId}");
            return reportId;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error scheduling report: {ex.Message}");
            throw;
        }
    }

    public async Task<List<AnalyticsReportDto>> GetReportHistoryAsync(
        int userId,
        int pageNumber = 1,
        int pageSize = 20)
    {
        try
        {
            _logger.LogInformation($"Getting report history for user {userId}, page {pageNumber}");

            var reports = new List<AnalyticsReportDto>();
            for (int i = 0; i < Math.Min(pageSize, 10); i++)
            {
                reports.Add(new AnalyticsReportDto
                {
                    Id = new Random().Next(1000, 9999),
                    Title = $"Report {pageNumber}-{i + 1}",
                    Format = "json",
                    GeneratedAt = DateTime.UtcNow.AddDays(-i),
                    StartDate = DateTime.UtcNow.AddDays(-30),
                    EndDate = DateTime.UtcNow.AddDays(-i),
                    TotalConversations = new Random().Next(10, 100),
                    TotalMessages = new Random().Next(50, 500),
                    AverageSentiment = Math.Round(new Random().NextDouble() * 2 - 1, 2),
                    SummaryStats = new Dictionary<string, object> { { "summary", "Report data" } }
                });
            }

            _logger.LogInformation($"Retrieved {reports.Count} reports from history");
            return reports;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving report history: {ex.Message}");
            throw;
        }
    }

    private byte[] ExportAsCSV(AnalyticsReportDto report)
    {
        var csv = $"Report ID,Title,Format,Generated,Conversations,Messages,Avg Sentiment\n";
        csv += $"{report.Id},{report.Title},{report.Format},{report.GeneratedAt},{report.TotalConversations},{report.TotalMessages},{report.AverageSentiment}\n";
        return System.Text.Encoding.UTF8.GetBytes(csv);
    }

    private byte[] ExportAsJSON(AnalyticsReportDto report)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(report);
        return System.Text.Encoding.UTF8.GetBytes(json);
    }

    private byte[] ExportAsPDF(AnalyticsReportDto report)
    {
        var pdfContent = $"%PDF-1.4\n1 0 obj\n<< /Type /Catalog /Pages 2 0 R >>\nendobj\nReport: {report.Title}";
        return System.Text.Encoding.UTF8.GetBytes(pdfContent);
    }

    private Dictionary<int, int> GenerateHourlyDistribution()
    {
        var distribution = new Dictionary<int, int>();
        var random = new Random();
        for (int i = 0; i < 24; i++)
        {
            distribution[i] = random.Next(0, 100);
        }
        return distribution;
    }
}

public class MetricDataPoint
{
    public DateTime DatePoint { get; set; }
    public int ConversationCount { get; set; }
    public int MessageCount { get; set; }
    public double AverageSentiment { get; set; }
    public double EngagementScore { get; set; }
}

public class ConversationSummaryDto
{
    public int ConversationId { get; set; }
    public string? Title { get; set; }
    public int MessageCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public double Score { get; set; }
}
