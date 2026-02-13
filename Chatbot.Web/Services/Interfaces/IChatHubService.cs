namespace Chatbot.Web.Services.Interfaces;

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
