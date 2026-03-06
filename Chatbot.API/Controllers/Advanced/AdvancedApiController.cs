using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Chatbot.API.Models.Requests;
using Chatbot.API.Models.Responses;
using Chatbot.API.Services.Core.Interfaces;
using Chatbot.API.Infrastructure.Facades.Interfaces;
using Chatbot.API.Infrastructure.Http.Interfaces;
using Chatbot.API.Infrastructure.Auth.Interfaces;
using Chatbot.API.Controllers.Chat;
using System.Security.Claims;

namespace Chatbot.API.Controllers.Advanced;

/// <summary>
/// Advanced APIs for conversation management, analytics, activity tracking, and data export
/// Provides comprehensive data management and reporting capabilities
/// </summary>
[ApiController]
[Route("api/v1/advanced")]
[Authorize]
public class AdvancedApiController : ApiControllerBase
{
    private readonly IConversationManagementService _conversationMgmt;
    private readonly IAnalyticsReportingService _analyticsReporting;
    private readonly IActivityTrackingService _activityTracking;
    private readonly ISystemMetricsService _systemMetrics;
    private readonly IAdvancedDataExportService _dataExport;
    private readonly ILogger<AdvancedApiController> _logger;

    public AdvancedApiController(
        IConversationManagementService conversationMgmt,
        IAnalyticsReportingService analyticsReporting,
        IActivityTrackingService activityTracking,
        ISystemMetricsService systemMetrics,
        IAdvancedDataExportService dataExport,
        IUserContextProvider userContextProvider,
        IApiResponseBuilder responseBuilder,
        ILogger<AdvancedApiController> logger)
        : base(userContextProvider, responseBuilder, logger)
    {
        _conversationMgmt = conversationMgmt;
        _analyticsReporting = analyticsReporting;
        _activityTracking = activityTracking;
        _systemMetrics = systemMetrics;
        _dataExport = dataExport;
        _logger = logger;
    }

    private int GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        return int.Parse(userIdClaim?.Value ?? "0");
    }

    #region Conversation Management

    /// <summary>
    /// Get conversations with advanced filtering and pagination
    /// </summary>
    /// <param name="request">Filter and pagination criteria</param>
    /// <returns>Paginated list of conversations</returns>
    [HttpPost("conversations/search")]
    [ProducesResponseType(typeof(PaginatedResponse<ConversationSummaryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetConversations([FromBody] ConversationFilterRequest request)
    {
        try
        {
            LogAction("GetConversations", new { pageNumber = request.PageNumber, pageSize = request.PageSize });
            var userId = GetUserId();
            var result = await _conversationMgmt.GetConversationsAsync(userId, request);
            return Ok(result, "Conversations retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving conversations");
            return StatusCode(500, new { message = "Error retrieving conversations", error = ex.Message });
        }
    }

    /// <summary>
    /// Get detailed conversation information
    /// </summary>
    [HttpGet("conversations/{conversationId:int}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetConversationDetail(int conversationId)
    {
        try
        {
            var userId = GetUserId();
            var result = await _conversationMgmt.GetConversationDetailAsync(conversationId, userId);
            if (result == null)
                return NotFound(new { message = "Conversation not found" });
            return Ok(result, "Conversation details retrieved");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving conversation detail");
            return StatusCode(500, new { message = "Error retrieving conversation", error = ex.Message });
        }
    }

    /// <summary>
    /// Search conversations by text
    /// </summary>
    [HttpGet("conversations/text-search")]
    [ProducesResponseType(typeof(List<ConversationSummaryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchConversations([FromQuery] string searchText)
    {
        try
        {
            var userId = GetUserId();
            var results = await _conversationMgmt.SearchConversationsAsync(userId, searchText);
            return Ok(results, $"Found {results.Count} conversations");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching conversations");
            return StatusCode(500, new { message = "Error searching conversations", error = ex.Message });
        }
    }

    /// <summary>
    /// Archive a conversation
    /// </summary>
    [HttpPut("conversations/{conversationId:int}/archive")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<IActionResult> ArchiveConversation(int conversationId)
    {
        try
        {
            var userId = GetUserId();
            var result = await _conversationMgmt.ArchiveConversationAsync(conversationId, userId);
            return Ok(result, result ? "Conversation archived" : "Failed to archive conversation");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error archiving conversation");
            return StatusCode(500, new { message = "Error archiving conversation", error = ex.Message });
        }
    }

    /// <summary>
    /// Restore an archived conversation
    /// </summary>
    [HttpPut("conversations/{conversationId:int}/restore")]
    public async Task<IActionResult> RestoreConversation(int conversationId)
    {
        try
        {
            var userId = GetUserId();
            var result = await _conversationMgmt.RestoreConversationAsync(conversationId, userId);
            return Ok(result, result ? "Conversation restored" : "Failed to restore conversation");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error restoring conversation");
            return StatusCode(500, new { message = "Error restoring conversation", error = ex.Message });
        }
    }

    /// <summary>
    /// Delete a conversation (soft delete)
    /// </summary>
    [HttpDelete("conversations/{conversationId:int}")]
    public async Task<IActionResult> DeleteConversation(int conversationId)
    {
        try
        {
            var userId = GetUserId();
            var result = await _conversationMgmt.DeleteConversationAsync(conversationId, userId);
            return Ok(result, result ? "Conversation deleted" : "Failed to delete conversation");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting conversation");
            return StatusCode(500, new { message = "Error deleting conversation", error = ex.Message });
        }
    }

    /// <summary>
    /// Update conversation tags
    /// </summary>
    [HttpPut("conversations/{conversationId:int}/tags")]
    public async Task<IActionResult> UpdateConversationTags(int conversationId, [FromBody] List<string> tags)
    {
        try
        {
            var userId = GetUserId();
            var result = await _conversationMgmt.UpdateConversationTagsAsync(conversationId, tags, userId);
            return Ok(result, "Tags updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tags");
            return StatusCode(500, new { message = "Error updating tags", error = ex.Message });
        }
    }

    /// <summary>
    /// Update conversation status
    /// </summary>
    [HttpPut("conversations/{conversationId:int}/status")]
    public async Task<IActionResult> UpdateConversationStatus(int conversationId, [FromBody] dynamic statusUpdate)
    {
        try
        {
            var userId = GetUserId();
            string status = statusUpdate.status;
            var result = await _conversationMgmt.UpdateConversationStatusAsync(conversationId, status, userId);
            return Ok(result, "Status updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating status");
            return StatusCode(500, new { message = "Error updating status", error = ex.Message });
        }
    }

    /// <summary>
    /// Perform bulk operations on multiple conversations
    /// </summary>
    [HttpPost("conversations/bulk-operation")]
    [ProducesResponseType(typeof(BulkOperationResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> BulkOperation([FromBody] BulkConversationOperationRequest request)
    {
        try
        {
            var userId = GetUserId();
            var result = await _conversationMgmt.PerformBulkOperationAsync(request, userId);
            return Ok(result, result.Status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing bulk operation");
            return StatusCode(500, new { message = "Error performing bulk operation", error = ex.Message });
        }
    }

    #endregion

    #region Analytics & Reporting

    /// <summary>
    /// Generate a custom analytics report
    /// </summary>
    [HttpPost("analytics/report")]
    [ProducesResponseType(typeof(AnalyticsReportDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GenerateReport([FromBody] AnalyticsReportRequest request)
    {
        try
        {
            var userId = GetUserId();
            var report = await _analyticsReporting.GenerateReportAsync(request, userId);
            return Ok(report, "Report generated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating report");
            return StatusCode(500, new { message = "Error generating report", error = ex.Message });
        }
    }

    /// <summary>
    /// Get summary statistics for date range
    /// </summary>
    [HttpGet("analytics/summary")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSummaryStats([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        try
        {
            var userId = GetUserId();
            var stats = await _analyticsReporting.GetSummaryStatsAsync(userId, startDate, endDate);
            return Ok(stats, "Summary statistics retrieved");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving summary stats");
            return StatusCode(500, new { message = "Error retrieving stats", error = ex.Message });
        }
    }

    /// <summary>
    /// Get time series metrics
    /// </summary>
    [HttpGet("analytics/timeseries")]
    [ProducesResponseType(typeof(List<MetricDataPoint>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMetricsTimeSeries(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        [FromQuery] string groupBy = "day")
    {
        try
        {
            var userId = GetUserId();
            var data = await _analyticsReporting.GetMetricsTimeSeriesAsync(userId, startDate, endDate, groupBy);
            return Ok(data, "Time series data retrieved");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving time series");
            return StatusCode(500, new { message = "Error retrieving time series", error = ex.Message });
        }
    }

    /// <summary>
    /// Get top conversations by metric
    /// </summary>
    [HttpGet("analytics/top-conversations")]
    [ProducesResponseType(typeof(List<ConversationSummaryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTopConversations(
        [FromQuery] string metricType,
        [FromQuery] int count = 10)
    {
        try
        {
            var userId = GetUserId();
            var results = await _analyticsReporting.GetTopConversationsAsync(userId, metricType, count);
            return Ok(results, $"Retrieved top {count} conversations");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving top conversations");
            return StatusCode(500, new { message = "Error retrieving conversations", error = ex.Message });
        }
    }

    #endregion

    #region Activity Tracking

    /// <summary>
    /// Get user activity log
    /// </summary>
    [HttpGet("activity/user")]
    [ProducesResponseType(typeof(List<ActivityLogDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserActivity(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50)
    {
        try
        {
            var userId = GetUserId();
            var activities = await _activityTracking.GetUserActivityAsync(userId, startDate, endDate, pageNumber, pageSize);
            return Ok(new { items = activities, count = activities.Count }, "Activity log retrieved");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving activity log");
            return StatusCode(500, new { message = "Error retrieving activity log", error = ex.Message });
        }
    }

    /// <summary>
    /// Get activity summary for user
    /// </summary>
    [HttpGet("activity/summary")]
    [ProducesResponseType(typeof(UserActivitySummaryDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActivitySummary(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        try
        {
            var userId = GetUserId();
            var summary = await _activityTracking.GetActivitySummaryAsync(userId, startDate, endDate);
            return Ok(summary, "Activity summary retrieved");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving activity summary");
            return StatusCode(500, new { message = "Error retrieving summary", error = ex.Message });
        }
    }

    #endregion

    #region System Metrics

    /// <summary>
    /// Get current system metrics and health status
    /// </summary>
    [HttpGet("metrics/current")]
    [ProducesResponseType(typeof(SystemMetricsDto), StatusCodes.Status200OK)]
    [AllowAnonymous]
    public async Task<IActionResult> GetCurrentMetrics()
    {
        try
        {
            var metrics = await _systemMetrics.GetCurrentMetricsAsync();
            return Ok(metrics, "Current metrics retrieved");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving metrics");
            return StatusCode(500, new { message = "Error retrieving metrics", error = ex.Message });
        }
    }

    /// <summary>
    /// Get system health status
    /// </summary>
    [HttpGet("metrics/health")]
    [AllowAnonymous]
    public async Task<IActionResult> GetHealthStatus()
    {
        try
        {
            var isHealthy = await _systemMetrics.PerformHealthCheckAsync();
            var status = await _systemMetrics.GetHealthStatusAsync();
            return Ok(new { isHealthy, status }, "Health check completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking health");
            return StatusCode(503, new { message = "System health check failed", isHealthy = false });
        }
    }

    /// <summary>
    /// Get API performance statistics
    /// </summary>
    [HttpGet("metrics/performance")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetPerformanceStats(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        try
        {
            var stats = await _systemMetrics.GetApiPerformanceStatsAsync(startDate, endDate);
            return Ok(stats, "Performance statistics retrieved");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving performance stats");
            return StatusCode(500, new { message = "Error retrieving stats", error = ex.Message });
        }
    }

    #endregion

    #region Data Export

    /// <summary>
    /// Export conversations to various formats
    /// </summary>
    [HttpPost("export/conversations")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ExportConversations([FromBody] Models.Requests.ExportConversationsRequest request)
    {
        try
        {
            var userId = GetUserId();
            var fileContent = await _dataExport.ExportConversationsAsync(request, userId);
            var contentType = GetContentType(request.Format);
            var fileName = $"{request.FileName}.{request.Format}";
            return File(fileContent, contentType, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting conversations");
            return StatusCode(500, new { message = "Error exporting conversations", error = ex.Message });
        }
    }

    /// <summary>
    /// Start async export job for large datasets
    /// </summary>
    [HttpPost("export/async")]
    public async Task<IActionResult> StartAsyncExport([FromBody] Models.Requests.ExportConversationsRequest request)
    {
        try
        {
            var userId = GetUserId();
            var jobId = await _dataExport.StartAsyncExportAsync(request, userId);
            return Accepted(new { jobId, message = "Export job started", statusUrl = $"/api/v1/advanced/export/status/{jobId}" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting export job");
            return StatusCode(500, new { message = "Error starting export job", error = ex.Message });
        }
    }

    /// <summary>
    /// Get export job status
    /// </summary>
    [HttpGet("export/status/{jobId}")]
    public async Task<IActionResult> GetExportStatus(string jobId)
    {
        try
        {
            var status = await _dataExport.GetExportJobStatusAsync(jobId);
            return Ok(status, "Job status retrieved");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving job status");
            return StatusCode(500, new { message = "Error retrieving status", error = ex.Message });
        }
    }

    #endregion

    #region Utility Methods

    private string GetContentType(string format)
    {
        return format.ToLower() switch
        {
            "csv" => "text/csv",
            "pdf" => "application/pdf",
            "json" => "application/json",
            "xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            _ => "application/octet-stream"
        };
    }

    #endregion
}
