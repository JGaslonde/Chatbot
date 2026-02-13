namespace Chatbot.Web.Services.Interfaces;

public interface IAuthService
{
    Task<(bool Success, string Message, string? Token)> LoginAsync(string username, string password);
    Task<(bool Success, string Message, string? Token)> RegisterAsync(string username, string email, string password);
    Task LogoutAsync();
    string? GetToken();
    bool IsAuthenticated();
}
