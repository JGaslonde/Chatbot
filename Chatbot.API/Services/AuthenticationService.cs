using Chatbot.API.Models.Entities;
using Chatbot.API.Data;

namespace Chatbot.API.Services;

public interface IAuthenticationService
{
    Task<(bool Success, string Token, string Message)> RegisterAsync(string username, string email, string password);
    Task<(bool Success, string Token, string Message)> LoginAsync(string username, string password);
    Task<User?> ValidateTokenAsync(string token);
    string GenerateToken(User user);
}

public class AuthenticationService : IAuthenticationService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;

    public AuthenticationService(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }

    public async Task<(bool Success, string Token, string Message)> RegisterAsync(string username, string email, string password)
    {
        // Check if user already exists
        var existingUser = await _userRepository.GetByUsernameAsync(username);
        if (existingUser != null)
            return (false, "", "Username already exists");

        var existingEmail = await _userRepository.GetByEmailAsync(email);
        if (existingEmail != null)
            return (false, "", "Email already exists");

        // Create new user
        var user = new User
        {
            Username = username,
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            DisplayName = username,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _userRepository.AddAsync(user);
        var token = GenerateToken(user);
        return (true, token, "User registered successfully");
    }

    public async Task<(bool Success, string Token, string Message)> LoginAsync(string username, string password)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        if (user == null)
            return (false, "", "Invalid credentials");

        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return (false, "", "Invalid credentials");

        user.LastActive = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);

        var token = GenerateToken(user);
        return (true, token, "Login successful");
    }

    public async Task<User?> ValidateTokenAsync(string token)
    {
        // Token validation would be implemented here
        return await Task.FromResult<User?>(null);
    }

    public string GenerateToken(User user)
    {
        // Basic token generation - in production, use JWT
        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{user.Id}:{user.Username}:{DateTime.UtcNow.AddHours(24)}"));
    }
}
