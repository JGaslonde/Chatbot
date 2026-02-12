using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;
using FluentAssertions;
using Chatbot.API.Data;
using Chatbot.API.Models.Requests;
using Chatbot.API.Models.Responses;

namespace Chatbot.API.Tests.Integration;

public class ChatApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public ChatApiIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
            
            builder.ConfigureServices(services =>
            {
                // Add in-memory database for testing
                services.AddDbContext<ChatbotDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDatabase_" + Guid.NewGuid().ToString());
                });
            });
        });

        _client = _factory.CreateClient();
        
        // Ensure database is created
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ChatbotDbContext>();
        db.Database.EnsureCreated();
    }

    [Fact]
    public async Task HealthCheck_ReturnsOk()
    {
        // Act
        var response = await _client.GetAsync("/api/chat/health");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("status");
    }

    [Fact]
    public async Task RegisterUser_WithValidData_ReturnsToken()
    {
        // Arrange
        var request = new CreateUserRequest(
            "testuser" + Guid.NewGuid().ToString().Substring(0, 8),
            $"test{Guid.NewGuid().ToString().Substring(0, 8)}@example.com",
            "SecurePass123!"
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/chat/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<AuthResponse>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task RegisterUser_WithInvalidPassword_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateUserRequest(
            "testuser",
            "test@example.com",
            "weak" // Too weak
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/chat/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateConversation_ReturnsConversation()
    {
        // Arrange
        var request = new StartConversationRequest("Test Conversation");

        // Act
        var response = await _client.PostAsJsonAsync("/api/chat/conversations", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<ConversationResponse>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().BeGreaterThan(0);
        result.Data.Title.Should().Be("Test Conversation");
    }

    [Fact]
    public async Task SendMessage_WithValidMessage_ReturnsResponse()
    {
        // Arrange
        // First create a conversation
        var conversationRequest = new StartConversationRequest("Test");
        var conversationResponse = await _client.PostAsJsonAsync("/api/chat/conversations", conversationRequest);
        var conversation = await conversationResponse.Content.ReadFromJsonAsync<ApiResponse<ConversationResponse>>();
        var conversationId = conversation!.Data!.Id;

        var messageRequest = new ChatMessageRequest("Hello, how are you?");

        // Act
        var response = await _client.PostAsJsonAsync($"/api/chat/{conversationId}/send", messageRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<ChatMessageResponse>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Message.Should().NotBeNullOrEmpty();
        result.Data.Sentiment.Should().NotBeNullOrEmpty();
        result.Data.Intent.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task SendMessage_WithEmptyMessage_ReturnsBadRequest()
    {
        // Arrange
        var conversationRequest = new StartConversationRequest("Test");
        var conversationResponse = await _client.PostAsJsonAsync("/api/chat/conversations", conversationRequest);
        var conversation = await conversationResponse.Content.ReadFromJsonAsync<ApiResponse<ConversationResponse>>();
        var conversationId = conversation!.Data!.Id;

        var messageRequest = new ChatMessageRequest(""); // Empty message

        // Act
        var response = await _client.PostAsJsonAsync($"/api/chat/{conversationId}/send", messageRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetConversationHistory_ReturnsMessages()
    {
        // Arrange
        var conversationRequest = new StartConversationRequest("Test");
        var conversationResponse = await _client.PostAsJsonAsync("/api/chat/conversations", conversationRequest);
        var conversation = await conversationResponse.Content.ReadFromJsonAsync<ApiResponse<ConversationResponse>>();
        var conversationId = conversation!.Data!.Id;

        // Send a message first
        var messageRequest = new ChatMessageRequest("Hello!");
        await _client.PostAsJsonAsync($"/api/chat/{conversationId}/send", messageRequest);

        // Act
        var response = await _client.GetAsync($"/api/chat/{conversationId}/history");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<MessageHistoryResponse>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Messages.Should().NotBeEmpty();
    }

    [Fact]
    public async Task SendMessage_WithTooLongMessage_ReturnsBadRequest()
    {
        // Arrange
        var conversationRequest = new StartConversationRequest("Test");
        var conversationResponse = await _client.PostAsJsonAsync("/api/chat/conversations", conversationRequest);
        var conversation = await conversationResponse.Content.ReadFromJsonAsync<ApiResponse<ConversationResponse>>();
        var conversationId = conversation!.Data!.Id;

        var messageRequest = new ChatMessageRequest(new string('a', 5001)); // Exceeds max length

        // Act
        var response = await _client.PostAsJsonAsync($"/api/chat/{conversationId}/send", messageRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RateLimiting_IncludesHeaders()
    {
        // Act
        var response = await _client.GetAsync("/api/chat/health");

        // Assert
        response.Headers.Should().ContainKey("X-RateLimit-Limit");
        response.Headers.Should().ContainKey("X-RateLimit-Remaining");
        response.Headers.Should().ContainKey("X-RateLimit-Reset");
    }
}
