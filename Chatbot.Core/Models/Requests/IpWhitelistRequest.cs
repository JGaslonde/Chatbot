namespace Chatbot.Core.Models.Requests;

public record IpWhitelistRequest(
    string IpAddress,
    string? Description,
    int? ExpirationDays = null
);
