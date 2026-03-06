using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Chatbot.API.Services.Phase3.Interfaces;
using Chatbot.Core.Models.Responses;

namespace Chatbot.API.Controllers;

/// <summary>
/// Phase 3 User Segmentation API endpoints
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class SegmentationController : ControllerBase
{
    private readonly IUserSegmentationService _segmentationService;
    private readonly ILogger<SegmentationController> _logger;

    public SegmentationController(IUserSegmentationService segmentationService, ILogger<SegmentationController> logger)
    {
        _segmentationService = segmentationService;
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
    /// Get user segment analysis
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<UserSegmentDto>> GetUserSegment()
    {
        try
        {
            var userId = GetUserId();
            var segment = await _segmentationService.GetUserSegmentAsync(userId);
            if (segment == null)
                return NotFound();

            return Ok(segment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user segment");
            return StatusCode(500, new { error = "Failed to retrieve segment" });
        }
    }

    /// <summary>
    /// Get segments by behavioral category
    /// </summary>
    [HttpGet("by-behavior/{behavior}")]
    public async Task<ActionResult<List<UserSegmentDto>>> GetSegmentsByBehavior(string behavior)
    {
        try
        {
            var segments = await _segmentationService.GetSegmentByBehaviorAsync(behavior);
            return Ok(segments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving segments by behavior");
            return StatusCode(500, new { error = "Failed to retrieve segments" });
        }
    }

    /// <summary>
    /// Get users at risk of churn
    /// </summary>
    [HttpGet("churn-risk")]
    public async Task<ActionResult<List<UserSegmentDto>>> GetChurnRiskUsers([FromQuery] double minRiskScore = 0.7)
    {
        try
        {
            var users = await _segmentationService.GetChurnRiskUsersAsync(minRiskScore);
            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving churn risk users");
            return StatusCode(500, new { error = "Failed to retrieve at-risk users" });
        }
    }

    /// <summary>
    /// Analyze user segment
    /// </summary>
    [HttpPost("analyze")]
    public async Task<ActionResult> AnalyzeSegment()
    {
        try
        {
            var userId = GetUserId();
            var analysis = await _segmentationService.AnalyzeUserSegmentAsync(userId);
            return Ok(analysis);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing user segment");
            return StatusCode(500, new { error = "Analysis failed" });
        }
    }

    /// <summary>
    /// Predict churn for user
    /// </summary>
    [HttpPost("predict-churn")]
    public async Task<ActionResult<ChurnPredictionDto>> PredictChurn()
    {
        try
        {
            var userId = GetUserId();
            var prediction = await _segmentationService.PredictChurnAsync(userId);
            return Ok(prediction);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error predicting churn");
            return StatusCode(500, new { error = "Churn prediction failed" });
        }
    }
}
