namespace Chatbot.Core.Models.Responses;

public record ApiKeyDto(
    int Id,
    string Name,
    string? Description,
    DateTime CreatedAt,
    DateTime? ExpiresAt,
    DateTime? LastUsedAt,
    bool IsActive
);
