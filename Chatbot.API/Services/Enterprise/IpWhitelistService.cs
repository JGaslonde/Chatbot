using Chatbot.API.Data.Repositories.Interfaces;
using Chatbot.API.Services.Core.Interfaces;
using Chatbot.Core.Models.Entities;
using Chatbot.Core.Models.Requests;
using Chatbot.Core.Models.Responses;

namespace Chatbot.API.Services.Core;

public class IpWhitelistService : IIpWhitelistService
{
    private readonly IIpWhitelistRepository _repository;
    private readonly ILogger<IpWhitelistService> _logger;

    public IpWhitelistService(IIpWhitelistRepository repository, ILogger<IpWhitelistService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<IpWhitelistDto> AddIpToWhitelistAsync(int userId, IpWhitelistRequest request)
    {
        try
        {
            var ipWhitelist = new IpWhitelist
            {
                UserId = userId,
                IpAddress = request.IpAddress,
                Description = request.Description,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = request.ExpirationDays.HasValue
                    ? DateTime.UtcNow.AddDays(request.ExpirationDays.Value)
                    : null,
                IsActive = true
            };

            await _repository.AddAsync(ipWhitelist);

            _logger.LogInformation($"IP {request.IpAddress} added to whitelist for user {userId}");

            return MapToDto(ipWhitelist);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error adding IP to whitelist: {ex.Message}");
            throw;
        }
    }

    public async Task<List<IpWhitelistDto>> GetUserWhitelistAsync(int userId)
    {
        var whitelist = await _repository.GetUserWhitelistAsync(userId);
        return whitelist.Select(MapToDto).ToList();
    }

    public async Task<bool> IsIpWhitelistedAsync(int userId, string ipAddress)
    {
        var whitelisted = await _repository.GetByIpAsync(userId, ipAddress);

        if (whitelisted == null)
            return false;

        // Check if expired
        if (whitelisted.ExpiresAt.HasValue && whitelisted.ExpiresAt < DateTime.UtcNow)
            return false;

        return true;
    }

    public async Task<bool> RemoveIpFromWhitelistAsync(int userId, int whitelistId)
    {
        var whitelist = await _repository.GetByIdAsync(whitelistId);
        if (whitelist == null || whitelist.UserId != userId)
            return false;

        await _repository.DeleteAsync(whitelistId);

        _logger.LogInformation($"IP removed from whitelist for user {userId}: {whitelistId}");
        return true;
    }

    private static IpWhitelistDto MapToDto(IpWhitelist whitelist)
    {
        return new IpWhitelistDto(
            whitelist.Id,
            whitelist.IpAddress,
            whitelist.Description,
            whitelist.CreatedAt,
            whitelist.ExpiresAt,
            whitelist.IsActive
        );
    }
}
