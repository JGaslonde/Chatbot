namespace Chatbot.Core.Models.Responses;

/// <summary>Response with search results</summary>
public record SearchResultDto(
    int Id,
    int ConversationId,
    string Content,
    string? Keywords,
    string? Topics,
    string? Intents,
    double RelevanceScore,
    DateTime CreatedAt
);
