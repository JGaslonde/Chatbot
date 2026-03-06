using Chatbot.API.Services.Core.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Chatbot.API.Services.Core;

public class CacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<CacheService> _logger;

    public CacheService(IDistributedCache cache, ILogger<CacheService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        try
        {
            var cachedValue = await _cache.GetStringAsync(key);
            if (string.IsNullOrEmpty(cachedValue))
                return default;

            return JsonSerializer.Deserialize<T>(cachedValue);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving from cache: {ex.Message}");
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        try
        {
            var json = JsonSerializer.Serialize(value);
            var options = new DistributedCacheEntryOptions();

            if (expiration.HasValue)
                options.AbsoluteExpirationRelativeToNow = expiration.Value;
            else
                options.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);

            await _cache.SetStringAsync(key, json, options);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error setting cache: {ex.Message}");
        }
    }

    public async Task RemoveAsync(string key)
    {
        try
        {
            await _cache.RemoveAsync(key);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error removing from cache: {ex.Message}");
        }
    }

    public async Task RemoveByPatternAsync(string pattern)
    {
        // Note: IDistributedCache doesn't support pattern matching
        // This is a simplified implementation
        _logger.LogWarning("Pattern-based cache removal is not supported by default DistributedCache");
        await Task.CompletedTask;
    }

    public async Task ClearAllAsync()
    {
        // Note: IDistributedCache doesn't support bulk clear
        // This would need a custom implementation
        _logger.LogWarning("Clear all cache is not supported by default DistributedCache");
        await Task.CompletedTask;
    }
}
