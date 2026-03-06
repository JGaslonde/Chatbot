using System.IdentityModel.Tokens.Jwt;
using Chatbot.API.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Chatbot.API.Middleware;

public class TokenRevocationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TokenRevocationMiddleware> _logger;

    public TokenRevocationMiddleware(RequestDelegate next, ILogger<TokenRevocationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, ChatbotDbContext dbContext)
    {
        // Only check bearer tokens on authenticated routes
        var authHeader = context.Request.Headers.Authorization.FirstOrDefault();
        if (authHeader != null && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            var token = authHeader["Bearer ".Length..].Trim();
            if (!string.IsNullOrEmpty(token))
            {
                var jti = ExtractJti(token);
                if (jti != null)
                {
                    var isRevoked = await dbContext.RevokedTokens
                        .AsNoTracking()
                        .AnyAsync(r => r.Jti == jti && r.ExpiresAt > DateTime.UtcNow);

                    if (isRevoked)
                    {
                        _logger.LogWarning("Revoked token used. Jti: {Jti}", jti);
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsJsonAsync(new { error = "Token has been revoked" });
                        return;
                    }
                }
            }
        }

        await _next(context);
    }

    private static string? ExtractJti(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            if (!handler.CanReadToken(token)) return null;
            var jwt = handler.ReadJwtToken(token);
            return jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
        }
        catch
        {
            return null;
        }
    }
}

public static class TokenRevocationMiddlewareExtensions
{
    public static IApplicationBuilder UseTokenRevocation(this IApplicationBuilder builder)
        => builder.UseMiddleware<TokenRevocationMiddleware>();
}
