using Chatbot.Core.Models.Requests;
using Chatbot.Core.Models.Responses;

namespace Chatbot.Web.Services;

public interface IApiKeyManagementService
{
    Task<ApiKeyCreateResponse?> GenerateApiKeyAsync(ApiKeyRequest request);
    Task<List<ApiKeyDto>> GetApiKeysAsync();
    Task<bool> RevokeApiKeyAsync(int apiKeyId);
}
