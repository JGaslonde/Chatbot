using System.Security.Claims;
using Chatbot.API.Exceptions;

namespace Chatbot.API.Infrastructure;

/// <summary>
/// Provides user context extraction from HTTP context.
/// Implements Dependency Inversion - Controllers depend on this abstraction, not directly on ClaimsPrincipal.
/// Applies DRY - User ID extraction logic centralized in one place.
/// </summary>
public interface IUserContextProvider
{
    int GetUserId();
    string GetUsername();
    bool TryGetUserId(out int userId);
}

public class UserContextProvider : IUserContextProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContextProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int GetUserId()
    {
        if (!TryGetUserId(out var userId))
            throw new UnauthorizedException("Invalid user token");
        return userId;
    }

    public string GetUsername()
    {
        var principal = _httpContextAccessor.HttpContext?.User
            ?? throw new UnauthorizedException("No user context available");

        return principal.FindFirst(ClaimTypes.Name)?.Value
            ?? principal.FindFirst("name")?.Value
            ?? throw new UnauthorizedException("Username not found in token");
    }

    public bool TryGetUserId(out int userId)
    {
        userId = 0;
        var principal = _httpContextAccessor.HttpContext?.User;

        if (principal == null)
            return false;

        var userIdClaim = principal.FindFirst("id")?.Value
            ?? principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return int.TryParse(userIdClaim, out userId);
    }
}
