using Chatbot.Core.Models.Responses;

namespace Chatbot.Web.Services;

public interface ITwoFactorManagementService
{
    Task<TwoFactorSetupResponse?> Setup2FAAsync();
    Task<bool> Enable2FAAsync(string secret, string verificationCode);
    Task<List<string>> Disable2FAAsync();
}
