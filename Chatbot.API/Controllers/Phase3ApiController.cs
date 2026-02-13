using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
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
            // TODO: Get userId from claims
            var userId = 1;
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
            // TODO: Get userId from claims
            var userId = 1;
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
            // TODO: Get userId from claims
            var userId = 1;
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
            // TODO: Get userId from claims
            var userId = 1;
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
    /// Get all ML insights for user
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<MLInsightDto>>> GetUserInsights()
    {
        try
        {
            // TODO: Get userId from claims
            var userId = 1;
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
            // TODO: Get userId from claims
            var userId = 1;
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
            // TODO: Get userId from claims
            var userId = 1;
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

/// <summary>
/// Phase 3 Workflow Automation API endpoints
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class WorkflowsController : ControllerBase
{
    private readonly IWorkflowService _workflowService;
    private readonly ILogger<WorkflowsController> _logger;

    public WorkflowsController(IWorkflowService workflowService, ILogger<WorkflowsController> logger)
    {
        _workflowService = workflowService;
        _logger = logger;
    }

    /// <summary>
    /// Get all workflows for user
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<WorkflowDefinitionDto>>> GetUserWorkflows()
    {
        try
        {
            // TODO: Get userId from claims
            var userId = 1;
            var workflows = await _workflowService.GetUserWorkflowsAsync(userId);
            return Ok(workflows);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving workflows");
            return StatusCode(500, new { error = "Failed to retrieve workflows" });
        }
    }

    /// <summary>
    /// Get specific workflow by ID
    /// </summary>
    [HttpGet("{workflowId}")]
    public async Task<ActionResult<WorkflowDefinitionDto>> GetWorkflow(int workflowId)
    {
        try
        {
            var workflow = await _workflowService.GetWorkflowAsync(workflowId);
            if (workflow == null)
                return NotFound();

            return Ok(workflow);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving workflow");
            return StatusCode(500, new { error = "Failed to retrieve workflow" });
        }
    }

    /// <summary>
    /// Create new workflow
    /// </summary>
    [HttpPost]
    public async Task<ActionResult> CreateWorkflow([FromBody] WorkflowDefinitionRequest request)
    {
        try
        {
            // TODO: Get userId from claims
            var userId = 1;
            var workflow = await _workflowService.CreateWorkflowAsync(userId, request);
            return CreatedAtAction(nameof(GetWorkflow), new { workflowId = workflow.Id }, workflow);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating workflow");
            return StatusCode(500, new { error = "Failed to create workflow" });
        }
    }

    /// <summary>
    /// Update existing workflow
    /// </summary>
    [HttpPut("{workflowId}")]
    public async Task<ActionResult> UpdateWorkflow(int workflowId, [FromBody] WorkflowDefinitionRequest request)
    {
        try
        {
            var workflow = await _workflowService.UpdateWorkflowAsync(workflowId, request);
            return Ok(workflow);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating workflow");
            return StatusCode(500, new { error = "Failed to update workflow" });
        }
    }

    /// <summary>
    /// Execute workflow immediately
    /// </summary>
    [HttpPost("{workflowId}/execute")]
    public async Task<ActionResult> ExecuteWorkflow(int workflowId, [FromBody] WorkflowExecutionRequest request)
    {
        try
        {
            var execution = await _workflowService.ExecuteWorkflowAsync(workflowId, request.ConversationId, request);
            return Accepted(execution);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing workflow");
            return StatusCode(500, new { error = "Failed to execute workflow" });
        }
    }

    /// <summary>
    /// Get workflow executions
    /// </summary>
    [HttpGet("{workflowId}/executions")]
    public async Task<ActionResult<List<WorkflowExecutionDto>>> GetWorkflowExecutions(int workflowId, [FromQuery] int limit = 50)
    {
        try
        {
            var executions = await _workflowService.GetWorkflowExecutionsAsync(workflowId, limit);
            return Ok(executions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving workflow executions");
            return StatusCode(500, new { error = "Failed to retrieve executions" });
        }
    }
}

/// <summary>
/// Phase 3 Advanced Search API endpoints
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class SearchController : ControllerBase
{
    private readonly ISearchService _searchService;
    private readonly ILogger<SearchController> _logger;

    public SearchController(ISearchService searchService, ILogger<SearchController> logger)
    {
        _searchService = searchService;
        _logger = logger;
    }

    /// <summary>
    /// Advanced search across conversations
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<SearchResultsPageDto>> Search([FromBody] SearchRequest request)
    {
        try
        {
            // TODO: Get userId from claims
            var userId = 1;
            var results = await _searchService.SearchAsync(userId, request);
            return Ok(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching");
            return StatusCode(500, new { error = "Search failed" });
        }
    }

    /// <summary>
    /// Search by content
    /// </summary>
    [HttpGet("content")]
    public async Task<ActionResult<List<SearchResultDto>>> SearchContent(
        [FromQuery] string query,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 20)
    {
        try
        {
            // TODO: Get userId from claims
            var userId = 1;
            var results = await _searchService.SearchContentAsync(userId, query, skip, take);
            return Ok(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching content");
            return StatusCode(500, new { error = "Search failed" });
        }
    }

    /// <summary>
    /// Search by topic
    /// </summary>
    [HttpGet("topics/{topic}")]
    public async Task<ActionResult<List<SearchResultDto>>> SearchByTopic(
        string topic,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 20)
    {
        try
        {
            // TODO: Get userId from claims
            var userId = 1;
            var results = await _searchService.SearchByTopicAsync(userId, topic, skip, take);
            return Ok(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching by topic");
            return StatusCode(500, new { error = "Search failed" });
        }
    }

    /// <summary>
    /// Search by intent
    /// </summary>
    [HttpGet("intents/{intent}")]
    public async Task<ActionResult<List<SearchResultDto>>> SearchByIntent(
        string intent,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 20)
    {
        try
        {
            // TODO: Get userId from claims
            var userId = 1;
            var results = await _searchService.SearchByIntentAsync(userId, intent, skip, take);
            return Ok(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching by intent");
            return StatusCode(500, new { error = "Search failed" });
        }
    }

    /// <summary>
    /// Rebuild search index for user
    /// </summary>
    [HttpPost("rebuild-index")]
    public async Task<ActionResult> RebuildSearchIndex()
    {
        try
        {
            // TODO: Get userId from claims
            var userId = 1;
            await _searchService.RebuildSearchIndexAsync(userId);
            return Accepted(new { message = "Index rebuild in progress" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rebuilding search index");
            return StatusCode(500, new { error = "Index rebuild failed" });
        }
    }
}

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
    /// Get user segment analysis
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<UserSegmentDto>> GetUserSegment()
    {
        try
        {
            // TODO: Get userId from claims
            var userId = 1;
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
            // TODO: Get userId from claims
            var userId = 1;
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
            // TODO: Get userId from claims
            var userId = 1;
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
