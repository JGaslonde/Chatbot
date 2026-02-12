using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Chatbot.Web.Services;

public class ApiAuthenticationStateProvider : AuthenticationStateProvider
{
    private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());
    private ClaimsPrincipal? _currentUser;

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        return Task.FromResult(new AuthenticationState(_currentUser ?? _anonymous));
    }

    public void MarkUserAsAuthenticated(string username, string token)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim("token", token)
        };
        var identity = new ClaimsIdentity(claims, "apiauth");
        _currentUser = new ClaimsPrincipal(identity);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public void MarkUserAsLoggedOut()
    {
        _currentUser = null;
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}
