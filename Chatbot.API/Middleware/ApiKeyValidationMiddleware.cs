using Chatbot.API.Services.Core.Interfaces;
using System.Security.Claims;

namespace Chatbot.API.Middleware;

public class ApiKeyValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiKeyValidationMiddleware> _logger;

    public ApiKeyValidationMiddleware(RequestDelegate next, ILogger<ApiKeyValidationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IApiKeyService apiKeyService)
    {
        // Only validate API keys for /api/v1/enterprise endpoints
        if (!context.Request.Path.StartsWithSegments("/api/v1/enterprise"))
        {
            await _next(context);
            return;
        }

        var apiKeyHeader = context.Request.Headers["X-API-Key"].ToString();

        if (string.IsNullOrWhiteSpace(apiKeyHeader))
        {
            _logger.LogWarning("Request to {Path} missing X-API-Key header", context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { error = "API key is required" });
            return;
        }

        try
        {
            var userId = await apiKeyService.GetUserIdByApiKeyAsync(apiKeyHeader);

            if (userId == null)
            {
                _logger.LogWarning("Invalid API key provided: {KeyPrefix}...", apiKeyHeader[..8]);
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { error = "Invalid API key" });
                return;
            }

            // Add user context to claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()!),
                new Claim(ClaimTypes.AuthenticationMethod, "ApiKey")
            };

            var identity = new ClaimsIdentity(claims, "ApiKey");
            context.User = new ClaimsPrincipal(identity);

            _logger.LogInformation("API key validated for user {UserId}", userId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error validating API key: {ex.Message}");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new { error = "An error occurred during authentication" });
            return;
        }

        await _next(context);
    }
}
