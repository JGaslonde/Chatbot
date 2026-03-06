using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Chatbot.API.Services.Phase3.Interfaces;
using Chatbot.Core.Models.Responses;

namespace Chatbot.API.Controllers;

/// <summary>
/// Phase 3 ML Insights API endpoints
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class InsightsController : ControllerBase
{
    private readonly IMLInsightService _insightService;
    private readonly ILogger<InsightsController> _logger;

    public InsightsController(IMLInsightService insightService, ILogger<InsightsController> logger)
    {
        _insightService = insightService;
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
    /// Get all ML insights for user
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<MLInsightDto>>> GetUserInsights()
    {
        try
        {
            var userId = GetUserId();
            var insights = await _insightService.GetUserInsightsAsync(userId);
            return Ok(insights);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving insights");
            return StatusCode(500, new { error = "Failed to retrieve insights" });
        }
    }

    /// <summary>
    /// Get insights by type
    /// </summary>
    [HttpGet("type/{insightType}")]
    public async Task<ActionResult<List<MLInsightDto>>> GetInsightsByType(string insightType)
    {
        try
        {
            var userId = GetUserId();
            var insights = await _insightService.GetByInsightTypeAsync(userId, insightType);
            return Ok(insights);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving insights by type");
            return StatusCode(500, new { error = "Failed to retrieve insights" });
        }
    }

    /// <summary>
    /// Get unreviewed insights
    /// </summary>
    [HttpGet("unreviewed")]
    public async Task<ActionResult<List<MLInsightDto>>> GetUnreviewedInsights()
    {
        try
        {
            var userId = GetUserId();
            var insights = await _insightService.GetUnreviewedInsightsAsync(userId);
            return Ok(insights);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving unreviewed insights");
            return StatusCode(500, new { error = "Failed to retrieve insights" });
        }
    }

    /// <summary>
    /// Mark insight as reviewed
    /// </summary>
    [HttpPut("{insightId}/review")]
    public async Task<ActionResult> MarkAsReviewed(int insightId)
    {
        try
        {
            var insight = await _insightService.MarkAsReviewedAsync(insightId);
            return Ok(insight);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking insight as reviewed");
            return StatusCode(500, new { error = "Failed to update insight" });
        }
    }
}
