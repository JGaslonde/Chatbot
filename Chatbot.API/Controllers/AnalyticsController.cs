using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Chatbot.API.Services.Phase3.Interfaces;
using Chatbot.Core.Models.Requests;
using Chatbot.Core.Models.Responses;

namespace Chatbot.API.Controllers;

/// <summary>
/// Phase 3 Advanced Analytics API endpoints
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class AnalyticsController : ControllerBase
{
    private readonly IConversationAnalyticsService _analyticsService;
    private readonly ILogger<AnalyticsController> _logger;

    public AnalyticsController(IConversationAnalyticsService analyticsService, ILogger<AnalyticsController> logger)
    {
        _analyticsService = analyticsService;
        _logger = logger;
    }

    /// <summary>
    /// Extract userId from JWT claims
    /// </summary>
    private int GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out var userId) ? userId : 0;
    }

    /// <summary>
    /// Get conversation analytics by ID
    /// </summary>
    [HttpGet("{conversationId}")]
    public async Task<ActionResult<ConversationAnalyticsDto>> GetConversationAnalytics(int conversationId)
    {
        try
        {
            var analytics = await _analyticsService.GetAnalyticsAsync(conversationId);
            if (analytics == null)
                return NotFound();

            return Ok(analytics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving conversation analytics");
            return StatusCode(500, new { error = "Failed to retrieve analytics" });
        }
    }

    /// <summary>
    /// Get user analytics summary for specified date range
    /// </summary>
    [HttpGet("summary")]
    public async Task<ActionResult<AnalyticsSummaryDto>> GetAnalyticsSummary([FromQuery] int days = 30)
    {
        try
        {
            var userId = GetUserId();
            var summary = await _analyticsService.GetAnalyticsSummaryAsync(userId, days);
            return Ok(summary);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving analytics summary");
            return StatusCode(500, new { error = "Failed to retrieve summary" });
        }
    }

    /// <summary>
    /// Get average sentiment score for user
    /// </summary>
    [HttpGet("sentiment")]
    public async Task<ActionResult<double>> GetAverageSentiment([FromQuery] int days = 30)
    {
        try
        {
            var userId = GetUserId();
            var avgSentiment = await _analyticsService.GetAverageSentimentAsync(userId, days);
            return Ok(new { avgSentiment });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving sentiment");
            return StatusCode(500, new { error = "Failed to retrieve sentiment" });
        }
    }

    /// <summary>
    /// Get analytics by date range
    /// </summary>
    [HttpGet("range")]
    public async Task<ActionResult<List<ConversationAnalyticsDto>>> GetAnalyticsByDateRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        try
        {
            var userId = GetUserId();
            var analytics = await _analyticsService.GetUserAnalyticsByDateRangeAsync(userId, startDate, endDate);
            return Ok(analytics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving analytics by date range");
            return StatusCode(500, new { error = "Failed to retrieve analytics" });
        }
    }

    /// <summary>
    /// Create or update conversation analytics
    /// </summary>
    [HttpPost]
    public async Task<ActionResult> CreateOrUpdateAnalytics(
        [FromQuery] int conversationId,
        [FromBody] AnalyticsRequest request)
    {
        try
        {
            var userId = GetUserId();
            var analytics = await _analyticsService.CreateOrUpdateAnalyticsAsync(conversationId, userId, request);
            return CreatedAtAction(nameof(GetConversationAnalytics), new { conversationId = analytics.Id }, analytics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating/updating analytics");
            return StatusCode(500, new { error = "Failed to save analytics" });
        }
    }
}
