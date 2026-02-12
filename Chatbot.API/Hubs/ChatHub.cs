using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using Chatbot.API.Services;
using Chatbot.Core.Models.Entities;

namespace Chatbot.API.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly IConversationService _conversationService;
    private readonly ILogger<ChatHub> _logger;

    public ChatHub(IConversationService conversationService, ILogger<ChatHub> logger)
    {
        _conversationService = conversationService;
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        var username = Context.User?.Identity?.Name ?? "Anonymous";
        _logger.LogInformation("User {Username} connected with connection ID {ConnectionId}", username, Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var username = Context.User?.Identity?.Name ?? "Anonymous";
        _logger.LogInformation("User {Username} disconnected", username);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task JoinConversation(int conversationId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"conversation_{conversationId}");
        _logger.LogInformation("Connection {ConnectionId} joined conversation {ConversationId}", Context.ConnectionId, conversationId);

        // Notify others in the conversation
        await Clients.OthersInGroup($"conversation_{conversationId}")
            .SendAsync("UserJoined", Context.User?.Identity?.Name ?? "Anonymous");
    }

    public async Task LeaveConversation(int conversationId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"conversation_{conversationId}");
        _logger.LogInformation("Connection {ConnectionId} left conversation {ConversationId}", Context.ConnectionId, conversationId);

        // Notify others in the conversation
        await Clients.OthersInGroup($"conversation_{conversationId}")
            .SendAsync("UserLeft", Context.User?.Identity?.Name ?? "Anonymous");
    }

    public async Task SendMessage(int conversationId, string message)
    {
        try
        {
            // Add user message
            var userMessage = await _conversationService.AddMessageAsync(conversationId, message, MessageSender.User);

            // Send user message to all clients in the conversation
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

            // Generate and send bot response
            var botResponseText = await _conversationService.GenerateBotResponseAsync(conversationId, message);
            var botMessage = await _conversationService.AddMessageAsync(conversationId, botResponseText, MessageSender.Bot);

            // Send bot message to all clients in the conversation
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending message in conversation {ConversationId}", conversationId);
            await Clients.Caller.SendAsync("Error", "Failed to send message");
        }
    }

    public async Task SendTypingIndicator(int conversationId, bool isTyping)
    {
        // Notify others in the conversation about typing status
        await Clients.OthersInGroup($"conversation_{conversationId}")
            .SendAsync("UserTyping", Context.User?.Identity?.Name ?? "Anonymous", isTyping);
    }
}
