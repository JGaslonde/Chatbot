using Chatbot.Core.Models.Entities;
using Chatbot.API.Data;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Chatbot.API.Services;

public interface IAuthenticationService
{
    Task<(bool Success, string Token, string Message, User? User)> RegisterAsync(string username, string email, string password);
    Task<(bool Success, string Token, string Message, User? User)> LoginAsync(string username, string password);
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

    public async Task<(bool Success, string Token, string Message, User? User)> RegisterAsync(string username, string email, string password)
    {
        // Check if user already exists
        var existingUser = await _userRepository.GetByUsernameAsync(username);
        if (existingUser != null)
            return (false, "", "Username already exists", null);

        var existingEmail = await _userRepository.GetByEmailAsync(email);
        if (existingEmail != null)
            return (false, "", "Email already exists", null);

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
        return (true, token, "User registered successfully", user);
    }

    public async Task<(bool Success, string Token, string Message, User? User)> LoginAsync(string username, string password)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        if (user == null)
            return (false, "", "Invalid credentials", null);

        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return (false, "", "Invalid credentials", null);

        if (!user.IsActive)
            return (false, "", "Account is inactive", null);

        user.LastActive = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);

        var token = GenerateToken(user);
        return (true, token, "Login successful", user);
    }

    public async Task<User?> ValidateTokenAsync(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured"));
            
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);
            
            return await _userRepository.GetByIdAsync(userId);
        }
        catch
        {
            return null;
        }
    }

    public string GenerateToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured"));
        
        // Parse expire minutes with proper error handling
        if (!double.TryParse(_configuration["Jwt:ExpireMinutes"], out double expireMinutes))
        {
            expireMinutes = 1440; // Default to 24 hours if parsing fails
        }
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("id", user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            }),
            Expires = DateTime.UtcNow.AddMinutes(expireMinutes),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
