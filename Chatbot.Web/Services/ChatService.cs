using Chatbot.Web.Models;
using System.Net.Http.Json;

namespace Chatbot.Web.Services;

public interface IChatService
{
    Task<(bool Success, string Message, ConversationResponse? Conversation)> StartConversationAsync(string? title = null);
    Task<(bool Success, string Message, ChatMessageResponse? Response)> SendMessageAsync(int conversationId, string message);
    Task<(bool Success, string Message, MessageHistoryResponse? History)> GetHistoryAsync(int conversationId);
}

public class ChatService : IChatService
{
    private readonly HttpClient _httpClient;

    public ChatService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<(bool Success, string Message, ConversationResponse? Conversation)> StartConversationAsync(string? title = null)
    {
        try
        {
            var request = new StartConversationRequest(title);
            var response = await _httpClient.PostAsJsonAsync("api/Chat/conversations", request);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<ConversationResponse>>();
                if (result?.Success == true)
                {
                    return (true, result.Message, result.Data);
                }
            }

            var errorResult = await response.Content.ReadFromJsonAsync<ApiResponse<ConversationResponse>>();
            return (false, errorResult?.Message ?? "Failed to start conversation", null);
        }
        catch (Exception ex)
        {
            return (false, $"Error: {ex.Message}", null);
        }
    }

    public async Task<(bool Success, string Message, ChatMessageResponse? Response)> SendMessageAsync(int conversationId, string message)
    {
        try
        {
            var request = new ChatMessageRequest(message);
            var response = await _httpClient.PostAsJsonAsync($"api/Chat/{conversationId}/send", request);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<ChatMessageResponse>>();
                if (result?.Success == true)
                {
                    return (true, result.Message, result.Data);
                }
            }

            var errorResult = await response.Content.ReadFromJsonAsync<ApiResponse<ChatMessageResponse>>();
            return (false, errorResult?.Message ?? "Failed to send message", null);
        }
        catch (Exception ex)
        {
            return (false, $"Error: {ex.Message}", null);
        }
    }

    public async Task<(bool Success, string Message, MessageHistoryResponse? History)> GetHistoryAsync(int conversationId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/Chat/{conversationId}/history");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<MessageHistoryResponse>>();
                if (result?.Success == true)
                {
                    return (true, result.Message, result.Data);
                }
            }

            var errorResult = await response.Content.ReadFromJsonAsync<ApiResponse<MessageHistoryResponse>>();
            return (false, errorResult?.Message ?? "Failed to get history", null);
        }
        catch (Exception ex)
        {
            return (false, $"Error: {ex.Message}", null);
        }
    }
}
