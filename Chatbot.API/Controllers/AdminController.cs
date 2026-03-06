using Chatbot.API.Services.Admin;
using Chatbot.API.Services.Admin.Interfaces;
using Chatbot.Core.Models.Requests;
using Chatbot.Core.Models.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chatbot.API.Controllers;

/// <summary>
/// Admin controller for system administration, user management, and audit logging.
/// </summary>
[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;
    private readonly IAuditService _auditService;
    private readonly ILogger<AdminController> _logger;

    public AdminController(
        IAdminService adminService,
        IAuditService auditService,
        ILogger<AdminController> logger)
    {
        _adminService = adminService;
        _auditService = auditService;
        _logger = logger;
    }

    /// <summary>
    /// Get system statistics and health information.
    /// </summary>
    [HttpGet("stats")]
    public async Task<ActionResult<SystemStatsDto>> GetSystemStats()
    {
        try
        {
            var stats = await _adminService.GetSystemStatsAsync();
            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting system stats: {ex.Message}");
            return StatusCode(500, new { error = "Failed to retrieve system statistics" });
        }
    }

    /// <summary>
    /// Get audit logs with optional filtering.
    /// </summary>
    [HttpGet("audit-logs")]
    public async Task<ActionResult<AuditLogResponse>> GetAuditLogs(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] string? userId = null,
        [FromQuery] string? action = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        try
        {
            var logs = await _auditService.GetAuditLogsAsync(
                page, pageSize, userId, action, startDate, endDate);
            return Ok(logs);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving audit logs: {ex.Message}");
            return StatusCode(500, new { error = "Failed to retrieve audit logs" });
        }
    }

    /// <summary>
    /// Get detailed audit log entry.
    /// </summary>
    [HttpGet("audit-logs/{id}")]
    public async Task<ActionResult<AuditLogEntry>> GetAuditLogEntry(int id)
    {
        try
        {
            var entry = await _auditService.GetAuditLogEntryAsync(id);
            if (entry == null)
                return NotFound(new { error = "Audit log entry not found" });

            return Ok(entry);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving audit log: {ex.Message}");
            return StatusCode(500, new { error = "Failed to retrieve audit log" });
        }
    }

    /// <summary>
    /// Clear old audit logs (retention policy).
    /// </summary>
    [HttpPost("audit-logs/cleanup")]
    public async Task<ActionResult> ClearOldAuditLogs(
        [FromQuery] int daysToKeep = 90)
    {
        try
        {
            var deletedCount = await _auditService.ClearOldAuditLogsAsync(daysToKeep);
            return Ok(new { message = $"Deleted {deletedCount} audit log entries", deletedCount });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error clearing audit logs: {ex.Message}");
            return StatusCode(500, new { error = "Failed to clear audit logs" });
        }
    }

    /// <summary>
    /// Get system configuration.
    /// </summary>
    [HttpGet("config")]
    public async Task<ActionResult<SystemConfigDto>> GetConfig()
    {
        try
        {
            var config = await _adminService.GetSystemConfigAsync();
            return Ok(config);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting system config: {ex.Message}");
            return StatusCode(500, new { error = "Failed to retrieve system configuration" });
        }
    }

    /// <summary>
    /// Update system configuration.
    /// </summary>
    [HttpPut("config")]
    public async Task<ActionResult<SystemConfigDto>> UpdateConfig(
        [FromBody] SystemConfigUpdateRequest request)
    {
        try
        {
            var updated = await _adminService.UpdateSystemConfigAsync(request);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating system config: {ex.Message}");
            return StatusCode(500, new { error = "Failed to update system configuration" });
        }
    }

    /// <summary>
    /// Get active users and sessions.
    /// </summary>
    [HttpGet("users/active")]
    public async Task<ActionResult<List<ActiveUserDto>>> GetActiveUsers()
    {
        try
        {
            var users = await _adminService.GetActiveUsersAsync();
            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting active users: {ex.Message}");
            return StatusCode(500, new { error = "Failed to retrieve active users" });
        }
    }

    /// <summary>
    /// Force logout for a user.
    /// </summary>
    [HttpPost("users/{userId}/logout")]
    public async Task<ActionResult> ForceLogout(int userId)
    {
        try
        {
            await _adminService.ForceUserLogoutAsync(userId);
            return Ok(new { message = "User logged out successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error forcing logout: {ex.Message}");
            return StatusCode(500, new { error = "Failed to logout user" });
        }
    }
}
