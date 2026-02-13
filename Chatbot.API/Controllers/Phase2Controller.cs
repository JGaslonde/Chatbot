using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Chatbot.API.Services.Core.Interfaces;
using Chatbot.Core.Models.Requests;
using Chatbot.Core.Models.Responses;

namespace Chatbot.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class Phase2Controller : ControllerBase
{
    private readonly IWebhookService _webhookService;
    private readonly IApiKeyService _apiKeyService;
    private readonly ITwoFactorService _twoFactorService;
    private readonly IIpWhitelistService _ipWhitelistService;
    private readonly IReportingService _reportingService;
    private readonly IImportService _importService;
    private readonly IUserPreferencesEnhancedService _preferencesService;
    private readonly ILogger<Phase2Controller> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public Phase2Controller(
        IWebhookService webhookService,
        IApiKeyService apiKeyService,
        ITwoFactorService twoFactorService,
        IIpWhitelistService ipWhitelistService,
        IReportingService reportingService,
        IImportService importService,
        IUserPreferencesEnhancedService preferencesService,
        ILogger<Phase2Controller> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _webhookService = webhookService;
        _apiKeyService = apiKeyService;
        _twoFactorService = twoFactorService;
        _ipWhitelistService = ipWhitelistService;
        _reportingService = reportingService;
        _importService = importService;
        _preferencesService = preferencesService;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    private int GetUserId()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        return int.Parse(userIdClaim?.Value ?? "0");
    }

    #region Webhooks

    /// <summary>
    /// Create a new webhook
    /// </summary>
    [HttpPost("webhooks")]
    public async Task<ActionResult<WebhookDto>> CreateWebhook([FromBody] WebhookRequest request)
    {
        try
        {
            var userId = GetUserId();
            var webhook = await _webhookService.CreateWebhookAsync(userId, request);
            return CreatedAtAction(nameof(GetWebhooks), new { id = webhook?.Id }, webhook);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error creating webhook: {ex.Message}");
            return StatusCode(500, new { message = "Error creating webhook" });
        }
    }

    /// <summary>
    /// Get all webhooks for the current user
    /// </summary>
    [HttpGet("webhooks")]
    public async Task<ActionResult<List<WebhookDto>>> GetWebhooks()
    {
        try
        {
            var userId = GetUserId();
            var webhooks = await _webhookService.GetUserWebhooksAsync(userId);
            return Ok(webhooks);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving webhooks: {ex.Message}");
            return StatusCode(500, new { message = "Error retrieving webhooks" });
        }
    }

    /// <summary>
    /// Update a webhook
    /// </summary>
    [HttpPut("webhooks/{webhookId:int}")]
    public async Task<ActionResult> UpdateWebhook(int webhookId, [FromBody] WebhookRequest request)
    {
        try
        {
            var userId = GetUserId();
            var success = await _webhookService.UpdateWebhookAsync(userId, webhookId, request);
            if (!success)
                return NotFound(new { message = "Webhook not found" });

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating webhook: {ex.Message}");
            return StatusCode(500, new { message = "Error updating webhook" });
        }
    }

    /// <summary>
    /// Delete a webhook
    /// </summary>
    [HttpDelete("webhooks/{webhookId:int}")]
    public async Task<ActionResult> DeleteWebhook(int webhookId)
    {
        try
        {
            var userId = GetUserId();
            var success = await _webhookService.DeleteWebhookAsync(userId, webhookId);
            if (!success)
                return NotFound(new { message = "Webhook not found" });

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting webhook: {ex.Message}");
            return StatusCode(500, new { message = "Error deleting webhook" });
        }
    }

    #endregion

    #region API Keys

    /// <summary>
    /// Generate a new API key
    /// </summary>
    [HttpPost("api-keys")]
    public async Task<ActionResult<ApiKeyCreateResponse>> GenerateApiKey([FromBody] ApiKeyRequest request)
    {
        try
        {
            var userId = GetUserId();
            var response = await _apiKeyService.GenerateApiKeyAsync(userId, request);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error generating API key: {ex.Message}");
            return StatusCode(500, new { message = "Error generating API key" });
        }
    }

    /// <summary>
    /// Get all API keys for the current user
    /// </summary>
    [HttpGet("api-keys")]
    public async Task<ActionResult<List<ApiKeyDto>>> GetApiKeys()
    {
        try
        {
            var userId = GetUserId();
            var keys = await _apiKeyService.GetUserApiKeysAsync(userId);
            return Ok(keys);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving API keys: {ex.Message}");
            return StatusCode(500, new { message = "Error retrieving API keys" });
        }
    }

    /// <summary>
    /// Revoke an API key
    /// </summary>
    [HttpDelete("api-keys/{apiKeyId:int}")]
    public async Task<ActionResult> RevokeApiKey(int apiKeyId)
    {
        try
        {
            var userId = GetUserId();
            var success = await _apiKeyService.RevokeApiKeyAsync(userId, apiKeyId);
            if (!success)
                return NotFound(new { message = "API key not found" });

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error revoking API key: {ex.Message}");
            return StatusCode(500, new { message = "Error revoking API key" });
        }
    }

    #endregion

    #region Two-Factor Authentication

    /// <summary>
    /// Generate 2FA setup (shows secret and QR code)
    /// </summary>
    [HttpPost("2fa/setup")]
    public async Task<ActionResult<TwoFactorSetupResponse>> Setup2FA()
    {
        try
        {
            var userId = GetUserId();
            var response = await _twoFactorService.GenerateTwoFactorSetupAsync(userId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error setting up 2FA: {ex.Message}");
            return StatusCode(500, new { message = "Error setting up 2FA" });
        }
    }

    /// <summary>
    /// Enable 2FA with verification code
    /// </summary>
    [HttpPost("2fa/enable")]
    public async Task<ActionResult> Enable2FA([FromBody] TwoFactorSetupRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Secret) || string.IsNullOrEmpty(request.VerificationCode))
                return BadRequest(new { message = "Secret and verification code are required" });

            var userId = GetUserId();
            var success = await _twoFactorService.EnableTwoFactorAsync(userId, request.Secret, request.VerificationCode);
            if (!success)
                return BadRequest(new { message = "Invalid verification code" });

            return Ok(new { message = "2FA enabled successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error enabling 2FA: {ex.Message}");
            return StatusCode(500, new { message = "Error enabling 2FA" });
        }
    }

    /// <summary>
    /// Disable 2FA (returns backup codes for reference)
    /// </summary>
    [HttpPost("2fa/disable")]
    public async Task<ActionResult> Disable2FA()
    {
        try
        {
            var userId = GetUserId();
            var backupCodes = await _twoFactorService.DisableTwoFactorAsync(userId);
            return Ok(new { message = "2FA disabled successfully", backupCodes });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error disabling 2FA: {ex.Message}");
            return StatusCode(500, new { message = "Error disabling 2FA" });
        }
    }

    #endregion

    #region IP Whitelist

    /// <summary>
    /// Add an IP to whitelist
    /// </summary>
    [HttpPost("ip-whitelist")]
    public async Task<ActionResult<IpWhitelistDto>> AddIpToWhitelist([FromBody] IpWhitelistRequest request)
    {
        try
        {
            var userId = GetUserId();
            var whitelist = await _ipWhitelistService.AddIpToWhitelistAsync(userId, request);
            return CreatedAtAction(nameof(GetWhitelist), whitelist);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error adding IP to whitelist: {ex.Message}");
            return StatusCode(500, new { message = "Error adding IP to whitelist" });
        }
    }

    /// <summary>
    /// Get IP whitelist for current user
    /// </summary>
    [HttpGet("ip-whitelist")]
    public async Task<ActionResult<List<IpWhitelistDto>>> GetWhitelist()
    {
        try
        {
            var userId = GetUserId();
            var whitelist = await _ipWhitelistService.GetUserWhitelistAsync(userId);
            return Ok(whitelist);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving whitelist: {ex.Message}");
            return StatusCode(500, new { message = "Error retrieving whitelist" });
        }
    }

    /// <summary>
    /// Remove IP from whitelist
    /// </summary>
    [HttpDelete("ip-whitelist/{whitelistId:int}")]
    public async Task<ActionResult> RemoveIpFromWhitelist(int whitelistId)
    {
        try
        {
            var userId = GetUserId();
            var success = await _ipWhitelistService.RemoveIpFromWhitelistAsync(userId, whitelistId);
            if (!success)
                return NotFound(new { message = "Whitelist entry not found" });

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error removing IP from whitelist: {ex.Message}");
            return StatusCode(500, new { message = "Error removing IP from whitelist" });
        }
    }

    #endregion

    #region Reports

    /// <summary>
    /// Create a scheduled report
    /// </summary>
    [HttpPost("reports")]
    public async Task<ActionResult<ScheduledReportDto>> CreateScheduledReport([FromBody] ScheduledReportRequest request)
    {
        try
        {
            var userId = GetUserId();
            var report = await _reportingService.CreateScheduledReportAsync(userId, request);
            return CreatedAtAction(nameof(GetReports), report);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error creating report: {ex.Message}");
            return StatusCode(500, new { message = "Error creating report" });
        }
    }

    /// <summary>
    /// Get all scheduled reports for current user
    /// </summary>
    [HttpGet("reports")]
    public async Task<ActionResult<List<ScheduledReportDto>>> GetReports()
    {
        try
        {
            var userId = GetUserId();
            var reports = await _reportingService.GetUserReportsAsync(userId);
            return Ok(reports);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving reports: {ex.Message}");
            return StatusCode(500, new { message = "Error retrieving reports" });
        }
    }

    /// <summary>
    /// Generate a report in specified format
    /// </summary>
    [HttpGet("reports/{reportId:int}/generate")]
    public async Task<ActionResult> GenerateReport(int reportId, [FromQuery] string format = "pdf")
    {
        try
        {
            var userId = GetUserId();
            var reportBytes = await _reportingService.GenerateReportAsync(userId, reportId, format);

            var contentType = format.ToLower() switch
            {
                "json" => "application/json",
                "csv" => "text/csv",
                "pdf" => "application/pdf",
                _ => "application/octet-stream"
            };

            return File(reportBytes, contentType, $"report_{reportId}.{format}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error generating report: {ex.Message}");
            return StatusCode(500, new { message = "Error generating report" });
        }
    }

    /// <summary>
    /// Update a scheduled report
    /// </summary>
    [HttpPut("reports/{reportId:int}")]
    public async Task<ActionResult> UpdateScheduledReport(int reportId, [FromBody] ScheduledReportRequest request)
    {
        try
        {
            var userId = GetUserId();
            var success = await _reportingService.UpdateScheduledReportAsync(userId, reportId, request);
            if (!success)
                return NotFound(new { message = "Report not found" });

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating report: {ex.Message}");
            return StatusCode(500, new { message = "Error updating report" });
        }
    }

    /// <summary>
    /// Delete a scheduled report
    /// </summary>
    [HttpDelete("reports/{reportId:int}")]
    public async Task<ActionResult> DeleteScheduledReport(int reportId)
    {
        try
        {
            var userId = GetUserId();
            var success = await _reportingService.DeleteScheduledReportAsync(userId, reportId);
            if (!success)
                return NotFound(new { message = "Report not found" });

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting report: {ex.Message}");
            return StatusCode(500, new { message = "Error deleting report" });
        }
    }

    #endregion

    #region Import

    /// <summary>
    /// Start a new bulk import job
    /// </summary>
    [HttpPost("imports")]
    public async Task<ActionResult<ImportJobDto>> StartImport([FromBody] StartImportRequest request)
    {
        try
        {
            var userId = GetUserId();
            var job = await _importService.StartImportAsync(userId, request);
            return CreatedAtAction(nameof(GetImportJob), new { jobId = job.Id }, job);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error starting import: {ex.Message}");
            return StatusCode(500, new { message = "Error starting import" });
        }
    }

    /// <summary>
    /// Get all import jobs for current user
    /// </summary>
    [HttpGet("imports")]
    public async Task<ActionResult<List<ImportJobDto>>> GetImportJobs()
    {
        try
        {
            var userId = GetUserId();
            var jobs = await _importService.GetUserImportJobsAsync(userId);
            return Ok(jobs);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving import jobs: {ex.Message}");
            return StatusCode(500, new { message = "Error retrieving import jobs" });
        }
    }

    /// <summary>
    /// Get import job status
    /// </summary>
    [HttpGet("imports/{jobId:int}")]
    public async Task<ActionResult<ImportJobDto>> GetImportJob(int jobId)
    {
        try
        {
            var userId = GetUserId();
            var job = await _importService.GetImportJobStatusAsync(userId, jobId);
            if (job == null)
                return NotFound(new { message = "Import job not found" });

            return Ok(job);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving import job: {ex.Message}");
            return StatusCode(500, new { message = "Error retrieving import job" });
        }
    }

    /// <summary>
    /// Upload chunk for import job
    /// </summary>
    [HttpPost("imports/{jobId:int}/upload")]
    public async Task<ActionResult> UploadChunk(int jobId)
    {
        try
        {
            using var memoryStream = new MemoryStream();
            await Request.Body.CopyToAsync(memoryStream);
            var chunk = memoryStream.ToArray();

            var success = await _importService.UploadChunkAsync(jobId, chunk);
            if (!success)
                return BadRequest(new { message = "Failed to upload chunk" });

            return Ok(new { message = "Chunk uploaded successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error uploading chunk: {ex.Message}");
            return StatusCode(500, new { message = "Error uploading chunk" });
        }
    }

    /// <summary>
    /// Process an import job
    /// </summary>
    [HttpPost("imports/{jobId:int}/process")]
    public async Task<ActionResult> ProcessImport(int jobId)
    {
        try
        {
            await _importService.ProcessImportAsync(jobId);
            return Ok(new { message = "Import processing started" });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error processing import: {ex.Message}");
            return StatusCode(500, new { message = "Error processing import" });
        }
    }

    #endregion

    #region User Preferences

    /// <summary>
    /// Get enhanced user preferences
    /// </summary>
    [HttpGet("preferences")]
    public async Task<ActionResult<EnhancedUserPreferencesDto>> GetPreferences()
    {
        try
        {
            var userId = GetUserId();
            var preferences = await _preferencesService.GetEnhancedPreferencesAsync(userId);
            return Ok(preferences);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving preferences: {ex.Message}");
            return StatusCode(500, new { message = "Error retrieving preferences" });
        }
    }

    /// <summary>
    /// Update enhanced user preferences
    /// </summary>
    [HttpPut("preferences")]
    public async Task<ActionResult<EnhancedUserPreferencesDto>> UpdatePreferences([FromBody] EnhancedUserPreferencesRequest request)
    {
        try
        {
            var userId = GetUserId();
            var preferences = await _preferencesService.UpdateEnhancedPreferencesAsync(userId, request);
            return Ok(preferences);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating preferences: {ex.Message}");
            return StatusCode(500, new { message = "Error updating preferences" });
        }
    }

    #endregion
}
