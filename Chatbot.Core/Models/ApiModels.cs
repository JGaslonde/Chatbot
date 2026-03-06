namespace Chatbot.Core.Models;

// This file has been refactored into individual files:
// Requests:
// - ChatMessageRequest.cs
// - CreateUserRequest.cs
// - LoginRequest.cs
// - StartConversationRequest.cs
//
// Responses:
// - ChatMessageResponse.cs
// - AuthResponse.cs
// - ConversationResponse.cs
// - MessageHistoryResponse.cs
// - MessageDto.cs
// - ApiResponse.cs
// - HealthResponse.cs
//
// For backward compatibility imports, see the Requests and Responses namespaces

<<<<<<< HEAD
=======
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

public record PaginatedResponse<T>(
    int Total,
    int Page,
    int PageSize,
    IEnumerable<T> Data
);
>>>>>>> fcfb8c252c839cf3d05dc028780147fd1ffddce7
