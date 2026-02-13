using Chatbot.Core.Models.Entities;

namespace Chatbot.API.Services.Core.Interfaces;

public interface IAuthenticationService
{
    Task<(bool Success, string Token, string Message, User? User)> RegisterAsync(string username, string email, string password);
    Task<(bool Success, string Token, string Message, User? User)> LoginAsync(string username, string password);
    Task<User?> ValidateTokenAsync(string token);
    string GenerateToken(User user);
}
