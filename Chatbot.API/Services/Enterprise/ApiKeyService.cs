using Chatbot.API.Data.Repositories.Interfaces;
using Chatbot.API.Services.Core.Interfaces;
using Chatbot.Core.Models.Entities;
using Chatbot.Core.Models.Requests;
using Chatbot.Core.Models.Responses;
using System.Security.Cryptography;
using System.Text;

namespace Chatbot.API.Services.Core;

public class ApiKeyService : IApiKeyService
{
    private readonly IApiKeyRepository _repository;
    private readonly ILogger<ApiKeyService> _logger;
    private const string KeyPrefix = "chb_";
    private const int KeyLength = 32;

    public ApiKeyService(IApiKeyRepository repository, ILogger<ApiKeyService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<ApiKeyCreateResponse> GenerateApiKeyAsync(int userId, ApiKeyRequest request)
    {
        try
        {
            // Generate a random key
            var key = GenerateRandomKey();
            var keyHash = HashApiKey(key);

            var apiKey = new ApiKey
            {
                UserId = userId,
                Name = request.Name,
                Description = request.Description,
                KeyHash = keyHash,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = request.ExpirationDays.HasValue
                    ? DateTime.UtcNow.AddDays(request.ExpirationDays.Value)
                    : null,
                IsActive = true
            };

            await _repository.AddAsync(apiKey);

            _logger.LogInformation($"API key created for user {userId}: {apiKey.Id}");

            return new ApiKeyCreateResponse(
                apiKey.Id,
                key,
                apiKey.Name,
                apiKey.CreatedAt
            );
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error generating API key: {ex.Message}");
            throw;
        }
    }

    public async Task<List<ApiKeyDto>> GetUserApiKeysAsync(int userId)
    {
        var keys = await _repository.GetUserApiKeysAsync(userId);
        return keys.Select(k => new ApiKeyDto(
            k.Id,
            k.Name,
            k.Description,
            k.CreatedAt,
            k.ExpiresAt,
            k.LastUsedAt,
            k.IsActive
        )).ToList();
    }

    public async Task<bool> ValidateApiKeyAsync(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return false;

        var hash = HashApiKey(key);
        var apiKey = await _repository.GetByHashAsync(hash);

        if (apiKey == null)
            return false;

        // Check expiration
        if (apiKey.ExpiresAt.HasValue && apiKey.ExpiresAt < DateTime.UtcNow)
            return false;

        // Update last used
        apiKey.LastUsedAt = DateTime.UtcNow;
        await _repository.UpdateAsync(apiKey);

        return true;
    }

    public async Task<int?> GetUserIdByApiKeyAsync(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return null;

        var hash = HashApiKey(key);
        var apiKey = await _repository.GetByHashAsync(hash);

        if (apiKey == null || (apiKey.ExpiresAt.HasValue && apiKey.ExpiresAt < DateTime.UtcNow))
            return null;

        apiKey.LastUsedAt = DateTime.UtcNow;
        await _repository.UpdateAsync(apiKey);

        return apiKey.UserId;
    }

    public async Task<bool> RevokeApiKeyAsync(int userId, int apiKeyId)
    {
        var apiKey = await _repository.GetByIdAsync(apiKeyId);
        if (apiKey == null || apiKey.UserId != userId)
            return false;

        apiKey.IsActive = false;
        await _repository.UpdateAsync(apiKey);

        _logger.LogInformation($"API key {apiKeyId} revoked for user {userId}");
        return true;
    }

    private static string GenerateRandomKey()
    {
        var randomBytes = new byte[KeyLength];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return KeyPrefix + Convert.ToBase64String(randomBytes).Replace("+", "-").Replace("/", "_");
    }

    private static string HashApiKey(string key)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(key));
        return Convert.ToBase64String(hashedBytes);
    }
}
