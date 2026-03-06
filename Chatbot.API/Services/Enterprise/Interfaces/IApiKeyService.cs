using Chatbot.Core.Models.Requests;
using Chatbot.Core.Models.Responses;

namespace Chatbot.API.Services.Core.Interfaces;

/// <summary>
/// Service for API key management.
/// </summary>
public interface IApiKeyService
{
    Task<ApiKeyCreateResponse> GenerateApiKeyAsync(int userId, ApiKeyRequest request);
    Task<List<ApiKeyDto>> GetUserApiKeysAsync(int userId);
    Task<bool> ValidateApiKeyAsync(string key);
    Task<int?> GetUserIdByApiKeyAsync(string key);
    Task<bool> RevokeApiKeyAsync(int userId, int apiKeyId);
}
