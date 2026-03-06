namespace Chatbot.Core.Models.Responses;

public record TwoFactorSetupResponse(
    string Secret,
    string QrCodeUrl,
    List<string> BackupCodes
);
