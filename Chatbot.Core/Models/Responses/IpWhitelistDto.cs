namespace Chatbot.Core.Models.Responses;

public record IpWhitelistDto(
    int Id,
    string IpAddress,
    string? Description,
    DateTime CreatedAt,
    DateTime? ExpiresAt,
    bool IsActive
);
