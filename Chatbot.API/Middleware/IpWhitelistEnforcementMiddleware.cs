using Chatbot.API.Services.Core.Interfaces;
using System.Net;

namespace Chatbot.API.Middleware;

public class IpWhitelistEnforcementMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<IpWhitelistEnforcementMiddleware> _logger;

    public IpWhitelistEnforcementMiddleware(RequestDelegate next, ILogger<IpWhitelistEnforcementMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IIpWhitelistService ipWhitelistService)
    {
        // Only enforce whitelist for /api/phase2 endpoints with authenticated users
        if (!context.Request.Path.StartsWithSegments("/api/phase2") || context.User?.Identity?.IsAuthenticated != true)
        {
            await _next(context);
            return;
        }

        try
        {
            // Get the user ID from claims
            var userIdClaim = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                await _next(context);
                return;
            }

            // Get client IP address
            var clientIp = GetClientIpAddress(context);

            if (string.IsNullOrWhiteSpace(clientIp))
            {
                _logger.LogWarning("Could not determine client IP for user {UserId}", userId);
                await _next(context);
                return;
            }

            // Check if IP is whitelisted
            var isWhitelisted = await ipWhitelistService.IsIpWhitelistedAsync(userId, clientIp);

            if (!isWhitelisted)
            {
                _logger.LogWarning("IP address {IpAddress} not whitelisted for user {UserId}", clientIp, userId);
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(new { error = "Your IP address is not whitelisted" });
                return;
            }

            _logger.LogInformation("IP {IpAddress} authenticated for user {UserId}", clientIp, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error enforcing IP whitelist: {ex.Message}");
            // Don't block on errors - log and continue
        }

        await _next(context);
    }

    private static string? GetClientIpAddress(HttpContext context)
    {
        // Check for IP from X-Forwarded-For header (proxies/load balancers)
        if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
        {
            var ips = forwardedFor.ToString().Split(',');
            if (ips.Length > 0 && !string.IsNullOrWhiteSpace(ips[0]))
            {
                return ips[0].Trim();
            }
        }

        // Check for X-Real-IP header (common with nginx)
        if (context.Request.Headers.TryGetValue("X-Real-IP", out var realIp))
        {
            if (!string.IsNullOrWhiteSpace(realIp))
            {
                return realIp.ToString();
            }
        }

        // Fall back to direct connection
        return context.Connection.RemoteIpAddress?.ToString();
    }
}
