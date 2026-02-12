using Microsoft.AspNetCore.SignalR.Client;

namespace Chatbot.Web.Services;

public interface IChatHubService
{
    Task StartAsync(string token);
    Task StopAsync();
    Task JoinConversationAsync(int conversationId);
    Task LeaveConversationAsync(int conversationId);
    Task SendMessageAsync(int conversationId, string message);
    Task SendTypingIndicatorAsync(int conversationId, bool isTyping);
    void OnMessageReceived(Action<object> handler);
    void OnUserJoined(Action<string> handler);
    void OnUserLeft(Action<string> handler);
    void OnUserTyping(Action<string, bool> handler);
    void OnError(Action<string> handler);
    bool IsConnected { get; }
}

public class ChatHubService : IChatHubService, IAsyncDisposable
{
    private HubConnection? _hubConnection;
    private readonly string _hubUrl;

    public ChatHubService(string apiBaseUrl)
    {
        _hubUrl = $"{apiBaseUrl.TrimEnd('/')}/chatHub";
    }

    public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;

    public async Task StartAsync(string token)
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(_hubUrl, options =>
            {
                options.AccessTokenProvider = () => Task.FromResult<string?>(token);
            })
            .WithAutomaticReconnect()
            .Build();

        await _hubConnection.StartAsync();
    }

    public async Task StopAsync()
    {
        if (_hubConnection != null)
        {
            await _hubConnection.StopAsync();
        }
    }

    public async Task JoinConversationAsync(int conversationId)
    {
        if (_hubConnection != null)
        {
            await _hubConnection.InvokeAsync("JoinConversation", conversationId);
        }
    }

    public async Task LeaveConversationAsync(int conversationId)
    {
        if (_hubConnection != null)
        {
            await _hubConnection.InvokeAsync("LeaveConversation", conversationId);
        }
    }

    public async Task SendMessageAsync(int conversationId, string message)
    {
        if (_hubConnection != null)
        {
            await _hubConnection.InvokeAsync("SendMessage", conversationId, message);
        }
    }

    public async Task SendTypingIndicatorAsync(int conversationId, bool isTyping)
    {
        if (_hubConnection != null)
        {
            await _hubConnection.InvokeAsync("SendTypingIndicator", conversationId, isTyping);
        }
    }

    public void OnMessageReceived(Action<object> handler)
    {
        _hubConnection?.On<object>("ReceiveMessage", handler);
    }

    public void OnUserJoined(Action<string> handler)
    {
        _hubConnection?.On<string>("UserJoined", handler);
    }

    public void OnUserLeft(Action<string> handler)
    {
        _hubConnection?.On<string>("UserLeft", handler);
    }

    public void OnUserTyping(Action<string, bool> handler)
    {
        _hubConnection?.On<string, bool>("UserTyping", handler);
    }

    public void OnError(Action<string> handler)
    {
        _hubConnection?.On<string>("Error", handler);
    }

    public async ValueTask DisposeAsync()
    {
        if (_hubConnection != null)
        {
            await _hubConnection.DisposeAsync();
        }
    }
}
