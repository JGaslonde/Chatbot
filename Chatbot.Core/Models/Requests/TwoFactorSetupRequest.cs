namespace Chatbot.Core.Models.Requests;

public record TwoFactorSetupRequest(
    string? Secret = null,
    string? VerificationCode = null
);
