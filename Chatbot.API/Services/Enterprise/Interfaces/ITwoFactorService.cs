using Chatbot.Core.Models.Responses;

namespace Chatbot.API.Services.Core.Interfaces;

/// <summary>
/// Service for two-factor authentication.
/// </summary>
public interface ITwoFactorService
{
    Task<TwoFactorSetupResponse> GenerateTwoFactorSetupAsync(int userId);
    Task<bool> VerifyTwoFactorAsync(int userId, string code);
    Task<bool> EnableTwoFactorAsync(int userId, string secret, string verificationCode);
    Task<List<string>> DisableTwoFactorAsync(int userId);
    Task<bool> VerifyTwoFactorLoginAsync(int userId, string code);
}
