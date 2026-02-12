using Microsoft.AspNetCore.Mvc;
using Chatbot.API.Services;
using Chatbot.API.Models.Requests;
using Chatbot.API.Models.Responses;
using Chatbot.API.Models.Entities;
using Chatbot.API.Exceptions;

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
        var (success, token, message) = await _authService.RegisterAsync(request.Username, request.Email, request.Password);
        if (!success)
            throw new ConflictException(message);

        return Ok(new ApiResponse<object>(true, message, new { token }));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var (success, token, message) = await _authService.LoginAsync(request.Username, request.Password);
        if (!success)
            throw new UnauthorizedException(message);

        return Ok(new ApiResponse<object>(true, message, new { token }));
    }

    [HttpPost("conversations")]
    public async Task<IActionResult> StartConversation([FromBody] StartConversationRequest request)
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

    [HttpPost("send")]
    [Route("{conversationId}/send")]
    public async Task<IActionResult> SendMessage(int conversationId, [FromBody] ChatMessageRequest request)
    {
        // Add user message
        var message = await _conversationService.AddMessageAsync(conversationId, request.Message, MessageSender.User);

        // Generate intelligent bot response using new template service
        var botResponseText = await _conversationService.GenerateBotResponseAsync(conversationId, request.Message);
        var botMessage = await _conversationService.AddMessageAsync(conversationId, botResponseText, MessageSender.Bot);

        // Update conversation summary periodically (every 5th message)
        if (conversationId % 5 == 0)
        {
            await _conversationService.UpdateConversationSummaryAsync(conversationId);
        }

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

    [HttpGet("{conversationId}/history")]
    public async Task<IActionResult> GetHistory(int conversationId)
    {
        var conversation = await _conversationService.GetConversationAsync(conversationId);
        if (conversation == null)
            throw new NotFoundException("Conversation", conversationId);

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

    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new HealthResponse("healthy", DateTime.UtcNow, "1.0.0"));
    }
}
