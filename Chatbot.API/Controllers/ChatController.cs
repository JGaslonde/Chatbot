using Microsoft.AspNetCore.Mvc;
using Chatbot.API.Services;
using Chatbot.API.Models.Requests;
using Chatbot.API.Models.Responses;
using Chatbot.API.Models.Entities;

namespace Chatbot.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly IConversationService _conversationService;
    private readonly IAuthenticationService _authService;
    private readonly ILogger<ChatController> _logger;

    public ChatController(
        IConversationService conversationService,
        IAuthenticationService authService,
        ILogger<ChatController> logger)
    {
        _conversationService = conversationService;
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] CreateUserRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest(new ApiResponse<object>(false, "Username and password are required", null, new List<string> { "Invalid input" }));

        var (success, token, message) = await _authService.RegisterAsync(request.Username, request.Email, request.Password);
        if (!success)
            return BadRequest(new ApiResponse<object>(false, message, null));

        return Ok(new ApiResponse<object>(true, message, new { token }));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest(new ApiResponse<object>(false, "Username and password are required", null, new List<string> { "Invalid input" }));

        var (success, token, message) = await _authService.LoginAsync(request.Username, request.Password);
        if (!success)
            return Unauthorized(new ApiResponse<object>(false, message, null));

        return Ok(new ApiResponse<object>(true, message, new { token }));
    }

    [HttpPost("conversations")]
    public async Task<IActionResult> StartConversation([FromBody] StartConversationRequest request)
    {
        try
        {
            // In a real app, you would extract the user ID from the JWT token
            var userId = 1; // Placeholder - should come from authenticated user
            var conversation = await _conversationService.CreateConversationAsync(userId, request.Title);

            var response = new ConversationResponse(
                conversation.Id,
                conversation.Title,
                conversation.StartedAt,
                0,
                conversation.Summary);

            return Ok(new ApiResponse<ConversationResponse>(true, "Conversation started", response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting conversation");
            return StatusCode(500, new ApiResponse<object>(false, "Error starting conversation", null));
        }
    }

    [HttpPost("send")]
    [Route("{conversationId}/send")]
    public async Task<IActionResult> SendMessage(int conversationId, [FromBody] ChatMessageRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
            return BadRequest(new ApiResponse<object>(false, "Message cannot be empty", null));

        try
        {
            var message = await _conversationService.AddMessageAsync(conversationId, request.Message, MessageSender.User);

            // Generate bot response
            var botResponse = $"Echo: {request.Message}"; // Placeholder
            var botMessage = await _conversationService.AddMessageAsync(conversationId, botResponse, MessageSender.Bot);

            var response = new ChatMessageResponse(
                botMessage.Content,
                botMessage.SentAt,
                botMessage.DetectedIntent ?? "unknown",
                botMessage.IntentConfidence,
                botMessage.Sentiment.ToString(),
                botMessage.SentimentScore,
                conversationId);

            return Ok(new ApiResponse<ChatMessageResponse>(true, "Message processed", response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing message");
            return StatusCode(500, new ApiResponse<object>(false, "Error processing message", null));
        }
    }

    [HttpGet("{conversationId}/history")]
    public async Task<IActionResult> GetHistory(int conversationId)
    {
        try
        {
            var conversation = await _conversationService.GetConversationAsync(conversationId);
            if (conversation == null)
                return NotFound(new ApiResponse<object>(false, "Conversation not found", null));

            var messageDtos = conversation.Messages
                .OrderBy(m => m.SentAt)
                .Select(m => new MessageDto(
                    m.Id,
                    m.Content,
                    m.Sender.ToString(),
                    m.SentAt,
                    m.Sentiment.ToString(),
                    m.DetectedIntent,
                    m.SentimentScore))
                .ToList();

            var response = new MessageHistoryResponse(conversationId, messageDtos);
            return Ok(new ApiResponse<MessageHistoryResponse>(true, "History retrieved", response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving history");
            return StatusCode(500, new ApiResponse<object>(false, "Error retrieving history", null));
        }
    }

    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new HealthResponse("healthy", DateTime.UtcNow, "1.0.0"));
    }
}
