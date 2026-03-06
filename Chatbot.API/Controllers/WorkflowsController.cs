using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Chatbot.API.Services.Phase3.Interfaces;
using Chatbot.Core.Models.Requests;
using Chatbot.Core.Models.Responses;

namespace Chatbot.API.Controllers;

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
    /// Extract userId from JWT claims
    /// </summary>
    private int GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out var userId) ? userId : 0;
    }

    /// <summary>
    /// Get all workflows for user
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<WorkflowDefinitionDto>>> GetUserWorkflows()
    {
        try
        {
            var userId = GetUserId();
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
            var userId = GetUserId();
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
