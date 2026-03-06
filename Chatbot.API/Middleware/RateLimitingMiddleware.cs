using System.Collections.Concurrent;
using System.Net;

namespace Chatbot.API.Middleware;

public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RateLimitingMiddleware> _logger;
    private readonly IConfiguration _configuration;

    // Store: key -> (RequestCount, WindowStart)
    private static readonly ConcurrentDictionary<string, (int Count, DateTime WindowStart)> _requestCounts = new();

    private static readonly Random _random = new();

    public RateLimitingMiddleware(RequestDelegate next, ILogger<RateLimitingMiddleware> logger, IConfiguration configuration)
    {
        _next = next;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var clientIp = GetClientIpAddress(context);

        if (string.IsNullOrEmpty(clientIp))
        {
            await _next(context);
            return;
        }

        var path = context.Request.Path.Value?.ToLowerInvariant() ?? string.Empty;
        var isAuthEndpoint = path.EndsWith("/login") || path.EndsWith("/register");

        var maxRequests = isAuthEndpoint
            ? _configuration.GetValue<int>("RateLimiting:AuthEndpointMaxRequests", 5)
            : _configuration.GetValue<int>("RateLimiting:GeneralEndpointMaxRequests", 100);

        var windowMinutes = _configuration.GetValue<int>("RateLimiting:WindowMinutes", 1);
        var windowDuration = TimeSpan.FromMinutes(windowMinutes);

        // Use separate key buckets for auth vs general endpoints
        var rateLimitKey = isAuthEndpoint ? $"auth:{clientIp}" : $"general:{clientIp}";
        var now = DateTime.UtcNow;

        var rateLimitInfo = _requestCounts.AddOrUpdate(
            rateLimitKey,
            _ => (1, now),
            (_, existing) =>
            {
                if (now - existing.WindowStart >= windowDuration)
                    return (1, now);
                return (existing.Count + 1, existing.WindowStart);
            });

        if (rateLimitInfo.Count > maxRequests)
        {
            var retryAfter = (int)(windowDuration - (now - rateLimitInfo.WindowStart)).TotalSeconds;

            _logger.LogWarning(
                "Rate limit exceeded for IP: {ClientIp} on {EndpointType} endpoint. Count: {Count}",
                clientIp, isAuthEndpoint ? "auth" : "general", rateLimitInfo.Count);

            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            context.Response.Headers["Retry-After"] = retryAfter.ToString();
            context.Response.Headers["X-RateLimit-Limit"] = maxRequests.ToString();
            context.Response.Headers["X-RateLimit-Remaining"] = "0";
            context.Response.Headers["X-RateLimit-Reset"] = rateLimitInfo.WindowStart.Add(windowDuration).ToString("o");

            await context.Response.WriteAsJsonAsync(new
            {
                error = "Rate limit exceeded",
                message = $"Too many requests. Please try again in {retryAfter} seconds.",
                retryAfter
            });

            return;
        }

        var remaining = maxRequests - rateLimitInfo.Count;
        context.Response.OnStarting(() =>
        {
            context.Response.Headers["X-RateLimit-Limit"] = maxRequests.ToString();
            context.Response.Headers["X-RateLimit-Remaining"] = Math.Max(0, remaining).ToString();
            context.Response.Headers["X-RateLimit-Reset"] = rateLimitInfo.WindowStart.Add(windowDuration).ToString("o");
            return Task.CompletedTask;
        });

        if (_random.Next(100) == 0)
            CleanupExpiredEntries(now, windowDuration);

        await _next(context);
    }

    private string GetClientIpAddress(HttpContext context)
    {
        // Only trust X-Forwarded-For if the direct connection is from a trusted proxy
        var trustedProxies = _configuration
            .GetSection("RateLimiting:TrustedProxies")
            .Get<string[]>() ?? [];

        var remoteIp = context.Connection.RemoteIpAddress?.ToString() ?? string.Empty;

        if (trustedProxies.Length > 0 && trustedProxies.Contains(remoteIp, StringComparer.OrdinalIgnoreCase))
        {
            // Trust X-Forwarded-For only from known proxies — take the last (rightmost) untrusted IP
            var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                var ips = forwardedFor.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                // Walk right-to-left to find first non-trusted IP
                for (int i = ips.Length - 1; i >= 0; i--)
                {
                    if (IPAddress.TryParse(ips[i], out _) && !trustedProxies.Contains(ips[i], StringComparer.OrdinalIgnoreCase))
                        return ips[i];
                }
            }
        }

        return remoteIp;
    }

    private void CleanupExpiredEntries(DateTime now, TimeSpan windowDuration)
    {
        var expiredKeys = _requestCounts
            .Where(kvp => now - kvp.Value.WindowStart >= windowDuration.Add(TimeSpan.FromMinutes(5)))
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var key in expiredKeys)
            _requestCounts.TryRemove(key, out _);

        if (expiredKeys.Count > 0)
            _logger.LogInformation("Cleaned up {Count} expired rate limit entries", expiredKeys.Count);
    }
}

public static class RateLimitingMiddlewareExtensions
{
    public static IApplicationBuilder UseRateLimiting(this IApplicationBuilder builder)
        => builder.UseMiddleware<RateLimitingMiddleware>();
}
