namespace Chatbot.Core.Models;

public record ChatMessageRequest(string Message, string? ConversationId = null);

public record CreateUserRequest(string Username, string Email, string Password);

public record LoginRequest(string Username, string Password);

public record StartConversationRequest(string? Title = null);

public record ChatMessageResponse(
    string Message,
    DateTime Timestamp,
    string Intent,
    double IntentConfidence,
    string Sentiment,
    double SentimentScore,
    int ConversationId
);

public record AuthResponse(
    string Token,
    string Username,
    string Email,
    DateTime ExpiresAt
);

public record ConversationResponse(
    int Id,
    string? Title,
    DateTime StartedAt,
    int MessageCount,
    string? Summary
);

public record MessageHistoryResponse(
    int ConversationId,
    List<MessageDto> Messages
);

public record MessageDto(
    int Id,
    string Content,
    string Sender,
    DateTime SentAt,
    string Sentiment,
    string? Intent,
    double SentimentScore
);

public record ApiResponse<T>(
    bool Success,
    string Message,
    T? Data,
    List<string>? Errors = null
);

public record HealthResponse(
    string Status,
    DateTime Timestamp,
    string Version
);
