using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using Chatbot.API.Services.Core;
using Chatbot.API.Infrastructure.Authorization;
using Chatbot.Core.Models.Entities;

namespace Chatbot.API.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly IConversationService _conversationService;
    private readonly IConversationAccessControl _accessControl;
    private readonly ILogger<ChatHub> _logger;

    public ChatHub(
        IConversationService conversationService,
        IConversationAccessControl accessControl,
        ILogger<ChatHub> logger)
    {
        _conversationService = conversationService;
        _accessControl = accessControl;
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        var username = Context.User?.Identity?.Name ?? "Unknown";
        _logger.LogInformation("User {Username} connected with connection ID {ConnectionId}", username, Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var username = Context.User?.Identity?.Name ?? "Unknown";
        _logger.LogInformation("User {Username} disconnected", username);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task JoinConversation(int conversationId)
    {
        var userId = GetUserId();
        await _accessControl.VerifyHubAccessAsync(conversationId, userId, Context.ConnectionId);

        await Groups.AddToGroupAsync(Context.ConnectionId, $"conversation_{conversationId}");
        _logger.LogInformation("Connection {ConnectionId} joined conversation {ConversationId}", Context.ConnectionId, conversationId);

        await Clients.OthersInGroup($"conversation_{conversationId}")
            .SendAsync("UserJoined", Context.User?.Identity?.Name);
    }

    public async Task LeaveConversation(int conversationId)
    {
        var userId = GetUserId();
        await _accessControl.VerifyHubAccessAsync(conversationId, userId, Context.ConnectionId);

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"conversation_{conversationId}");
        _logger.LogInformation("Connection {ConnectionId} left conversation {ConversationId}", Context.ConnectionId, conversationId);

        await Clients.OthersInGroup($"conversation_{conversationId}")
            .SendAsync("UserLeft", Context.User?.Identity?.Name);
    }

    public async Task SendMessage(int conversationId, string message)
    {
        var userId = GetUserId();

        // Validate input before touching the DB
        if (string.IsNullOrWhiteSpace(message))
            throw new HubException("Message cannot be empty.");
        if (message.Length > 5000)
            throw new HubException("Message cannot exceed 5000 characters.");

        try
        {
            await _accessControl.VerifyHubAccessAsync(conversationId, userId, Context.ConnectionId);

            var userMessage = await _conversationService.AddMessageAsync(conversationId, message, MessageSender.User);

            await Clients.Group($"conversation_{conversationId}")
                .SendAsync("ReceiveMessage", new
                {
                    userMessage.Id,
                    userMessage.Content,
                    Sender = userMessage.Sender.ToString(),
                    userMessage.SentAt,
                    Sentiment = userMessage.Sentiment.ToString(),
                    userMessage.SentimentScore,
                    userMessage.DetectedIntent,
                    userMessage.IntentConfidence
                });

            var botResponseText = await _conversationService.GenerateBotResponseAsync(conversationId, message);
            var botMessage = await _conversationService.AddMessageAsync(conversationId, botResponseText, MessageSender.Bot);

            await Clients.Group($"conversation_{conversationId}")
                .SendAsync("ReceiveMessage", new
                {
                    botMessage.Id,
                    botMessage.Content,
                    Sender = botMessage.Sender.ToString(),
                    botMessage.SentAt,
                    Sentiment = botMessage.Sentiment.ToString(),
                    botMessage.SentimentScore,
                    botMessage.DetectedIntent,
                    botMessage.IntentConfidence
                });
        }
        catch (HubException)
        {
            throw; // Re-throw HubExceptions (access denied, validation)
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending message in conversation {ConversationId}", conversationId);
            await Clients.Caller.SendAsync("Error", "Failed to send message");
        }
    }

    public async Task SendTypingIndicator(int conversationId, bool isTyping)
    {
        var userId = GetUserId();
        await _accessControl.VerifyHubAccessAsync(conversationId, userId, Context.ConnectionId);

        await Clients.OthersInGroup($"conversation_{conversationId}")
            .SendAsync("UserTyping", Context.User?.Identity?.Name, isTyping);
    }

    private int GetUserId()
    {
        var idClaim = Context.User?.FindFirst("id")?.Value;
        if (idClaim == null || !int.TryParse(idClaim, out var userId))
            throw new HubException("Unauthorized: invalid user identity.");
        return userId;
    }
}
