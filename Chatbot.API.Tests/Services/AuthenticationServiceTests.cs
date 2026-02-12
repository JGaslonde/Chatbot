using Xunit;
using FluentAssertions;
using Moq;
using Microsoft.Extensions.Configuration;
using Chatbot.API.Services;
using Chatbot.API.Data;
using Chatbot.API.Models.Entities;

namespace Chatbot.API.Tests.Services;

public class AuthenticationServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly AuthenticationService _service;

    public AuthenticationServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockConfiguration = new Mock<IConfiguration>();
        
        // Set up JWT configuration
        _mockConfiguration.Setup(c => c["Jwt:Key"]).Returns("test-secret-key-that-is-long-enough-for-security");
        _mockConfiguration.Setup(c => c["Jwt:Issuer"]).Returns("test-issuer");
        _mockConfiguration.Setup(c => c["Jwt:Audience"]).Returns("test-audience");
        
        _service = new AuthenticationService(_mockUserRepository.Object, _mockConfiguration.Object);
    }

    [Fact]
    public async Task RegisterUser_WithValidCredentials_CreatesUser()
    {
        // Arrange
        var username = "testuser";
        var email = "test@example.com";
        var password = "SecurePass123!";

        _mockUserRepository
            .Setup(r => r.GetByUsernameAsync(username))
            .ReturnsAsync((User?)null);
        
        _mockUserRepository
            .Setup(r => r.GetByEmailAsync(email))
            .ReturnsAsync((User?)null);

        _mockUserRepository
            .Setup(r => r.AddAsync(It.IsAny<User>()))
            .ReturnsAsync((User u) => u);

        // Act
        var result = await _service.RegisterAsync(username, email, password);

        // Assert
        result.Success.Should().BeTrue();
        result.Token.Should().NotBeNullOrEmpty();
        _mockUserRepository.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task RegisterUser_WithExistingUsername_ReturnsFailure()
    {
        // Arrange
        var username = "existinguser";
        var email = "new@example.com";
        var password = "SecurePass123!";

        var existingUser = new User
        {
            Id = 1,
            Username = username,
            Email = "existing@example.com",
            PasswordHash = "hash"
        };

        _mockUserRepository
            .Setup(r => r.GetByUsernameAsync(username))
            .ReturnsAsync(existingUser);

        // Act
        var result = await _service.RegisterAsync(username, email, password);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("already exists");
        _mockUserRepository.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task RegisterUser_WithExistingEmail_ReturnsFailure()
    {
        // Arrange
        var username = "newuser";
        var email = "existing@example.com";
        var password = "SecurePass123!";

        var existingUser = new User
        {
            Id = 1,
            Username = "existinguser",
            Email = email,
            PasswordHash = "hash"
        };

        _mockUserRepository
            .Setup(r => r.GetByUsernameAsync(username))
            .ReturnsAsync((User?)null);
        
        _mockUserRepository
            .Setup(r => r.GetByEmailAsync(email))
            .ReturnsAsync(existingUser);

        // Act
        var result = await _service.RegisterAsync(username, email, password);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("already exists");
        _mockUserRepository.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsToken()
    {
        // Arrange
        var username = "testuser";
        var password = "SecurePass123!";
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

        var user = new User
        {
            Id = 1,
            Username = username,
            Email = "test@example.com",
            PasswordHash = passwordHash,
            IsActive = true
        };

        _mockUserRepository
            .Setup(r => r.GetByUsernameAsync(username))
            .ReturnsAsync(user);

        // Act
        var result = await _service.LoginAsync(username, password);

        // Assert
        result.Success.Should().BeTrue();
        result.Token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Login_WithInvalidUsername_ReturnsFailure()
    {
        // Arrange
        var username = "nonexistent";
        var password = "password";

        _mockUserRepository
            .Setup(r => r.GetByUsernameAsync(username))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _service.LoginAsync(username, password);

        // Assert
        result.Success.Should().BeFalse();
    }

    [Fact]
    public async Task Login_WithInvalidPassword_ReturnsFailure()
    {
        // Arrange
        var username = "testuser";
        var correctPassword = "CorrectPass123!";
        var wrongPassword = "WrongPass123!";
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(correctPassword);

        var user = new User
        {
            Id = 1,
            Username = username,
            Email = "test@example.com",
            PasswordHash = passwordHash,
            IsActive = true
        };

        _mockUserRepository
            .Setup(r => r.GetByUsernameAsync(username))
            .ReturnsAsync(user);

        // Act
        var result = await _service.LoginAsync(username, wrongPassword);

        // Assert
        result.Success.Should().BeFalse();
    }

    [Fact]
    public async Task Login_WithInactiveUser_ReturnsFailure()
    {
        // Arrange
        var username = "inactiveuser";
        var password = "SecurePass123!";
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

        var user = new User
        {
            Id = 1,
            Username = username,
            Email = "test@example.com",
            PasswordHash = passwordHash,
            IsActive = false
        };

        _mockUserRepository
            .Setup(r => r.GetByUsernameAsync(username))
            .ReturnsAsync(user);

        // Act
        var result = await _service.LoginAsync(username, password);

        // Assert
        result.Success.Should().BeFalse();
    }
}
