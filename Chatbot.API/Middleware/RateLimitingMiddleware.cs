using System.Collections.Concurrent;

namespace Chatbot.API.Middleware;

public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RateLimitingMiddleware> _logger;
    
    // Store: IP -> (RequestCount, WindowStart)
    private static readonly ConcurrentDictionary<string, (int Count, DateTime WindowStart)> _requestCounts = new();
    
    // Configuration
    private const int MaxRequestsPerWindow = 100;
    private static readonly TimeSpan WindowDuration = TimeSpan.FromMinutes(1);
    
    public RateLimitingMiddleware(RequestDelegate next, ILogger<RateLimitingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var clientIp = GetClientIpAddress(context);
        
        if (string.IsNullOrEmpty(clientIp))
        {
            await _next(context);
            return;
        }

        var now = DateTime.UtcNow;
        
        // Get or create rate limit info for this IP
        var rateLimitInfo = _requestCounts.AddOrUpdate(
            clientIp,
            _ => (1, now),
            (_, existing) =>
            {
                // Reset window if expired
                if (now - existing.WindowStart >= WindowDuration)
                {
                    return (1, now);
                }
                return (existing.Count + 1, existing.WindowStart);
            });

        // Check if rate limit exceeded
        if (rateLimitInfo.Count > MaxRequestsPerWindow)
        {
            var retryAfter = (int)(WindowDuration - (now - rateLimitInfo.WindowStart)).TotalSeconds;
            
            _logger.LogWarning("Rate limit exceeded for IP: {ClientIp}. Request count: {Count}", 
                clientIp, rateLimitInfo.Count);
            
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            context.Response.Headers["Retry-After"] = retryAfter.ToString();
            context.Response.Headers["X-RateLimit-Limit"] = MaxRequestsPerWindow.ToString();
            context.Response.Headers["X-RateLimit-Remaining"] = "0";
            context.Response.Headers["X-RateLimit-Reset"] = rateLimitInfo.WindowStart.Add(WindowDuration).ToString("o");
            
            await context.Response.WriteAsJsonAsync(new
            {
                error = "Rate limit exceeded",
                message = $"Too many requests. Please try again in {retryAfter} seconds.",
                retryAfter = retryAfter
            });
            
            return;
        }

        // Add rate limit headers to response
        var remaining = MaxRequestsPerWindow - rateLimitInfo.Count;
        context.Response.OnStarting(() =>
        {
            context.Response.Headers["X-RateLimit-Limit"] = MaxRequestsPerWindow.ToString();
            context.Response.Headers["X-RateLimit-Remaining"] = Math.Max(0, remaining).ToString();
            context.Response.Headers["X-RateLimit-Reset"] = rateLimitInfo.WindowStart.Add(WindowDuration).ToString("o");
            return Task.CompletedTask;
        });

        // Cleanup old entries periodically (simple approach)
        if (_random.Next(100) == 0) // 1% chance to cleanup
        {
            CleanupExpiredEntries(now);
        }

        await _next(context);
    }

    private string GetClientIpAddress(HttpContext context)
    {
        // Try to get real IP from common proxy headers
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            var ips = forwardedFor.Split(',', StringSplitOptions.RemoveEmptyEntries);
            if (ips.Length > 0)
                return ips[0].Trim();
        }

        var realIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(realIp))
            return realIp;

        return context.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
    }

    private static readonly Random _random = new();
    
    private void CleanupExpiredEntries(DateTime now)
    {
        var expiredKeys = _requestCounts
            .Where(kvp => now - kvp.Value.WindowStart >= WindowDuration.Add(TimeSpan.FromMinutes(5)))
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var key in expiredKeys)
        {
            _requestCounts.TryRemove(key, out _);
        }

        if (expiredKeys.Count > 0)
        {
            _logger.LogInformation("Cleaned up {Count} expired rate limit entries", expiredKeys.Count);
        }
    }
}

public static class RateLimitingMiddlewareExtensions
{
    public static IApplicationBuilder UseRateLimiting(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RateLimitingMiddleware>();
    }
}
