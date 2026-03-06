using Chatbot.API.Data;
using Chatbot.API.Services.Core.Interfaces;
using Chatbot.Core.Models.Requests;
using Chatbot.Core.Models.Responses;
using Microsoft.EntityFrameworkCore;

namespace Chatbot.API.Services.Core;

public class UserPreferencesEnhancedService : IUserPreferencesEnhancedService
{
    private readonly ChatbotDbContext _context;
    private readonly ILogger<UserPreferencesEnhancedService> _logger;

    public UserPreferencesEnhancedService(ChatbotDbContext context, ILogger<UserPreferencesEnhancedService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<EnhancedUserPreferencesDto> GetEnhancedPreferencesAsync(int userId)
    {
        try
        {
            var userPreferences = await _context.UserPreferences
                .FirstOrDefaultAsync(up => up.UserId == userId);

            if (userPreferences == null)
            {
                // Return default preferences
                return new EnhancedUserPreferencesDto(
                    userId,
                    "light",
                    "en",
                    true,
                    true,
                    "UTC",
                    null
                );
            }

            return new EnhancedUserPreferencesDto(
                userId,
                userPreferences.Theme,
                userPreferences.Language,
                userPreferences.EmailNotifications,
                userPreferences.PushNotifications,
                userPreferences.TimeZone,
                null // No custom settings in current model
            );
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting preferences: {ex.Message}");
            throw;
        }
    }

    public async Task<EnhancedUserPreferencesDto> UpdateEnhancedPreferencesAsync(int userId, EnhancedUserPreferencesRequest request)
    {
        try
        {
            var userPreferences = await _context.UserPreferences
                .FirstOrDefaultAsync(up => up.UserId == userId);

            if (userPreferences == null)
            {
                // If preferences don't exist, we can't create them without a User reference
                _logger.LogWarning($"User preferences not found for user {userId}");
                return new EnhancedUserPreferencesDto(
                    userId,
                    request.Theme ?? "light",
                    request.Language ?? "en",
                    request.EnableEmailNotifications ?? true,
                    request.EnablePushNotifications ?? true,
                    request.TimeZone ?? "UTC",
                    request.CustomSettings
                );
            }

            // Update existing preferences
            if (!string.IsNullOrEmpty(request.Theme))
                userPreferences.Theme = request.Theme;

            if (!string.IsNullOrEmpty(request.Language))
                userPreferences.Language = request.Language;

            if (!string.IsNullOrEmpty(request.TimeZone))
                userPreferences.TimeZone = request.TimeZone;

            if (request.EnableEmailNotifications.HasValue)
                userPreferences.EmailNotifications = request.EnableEmailNotifications.Value;

            if (request.EnablePushNotifications.HasValue)
                userPreferences.PushNotifications = request.EnablePushNotifications.Value;

            userPreferences.UpdatedAt = DateTime.UtcNow;

            _context.UserPreferences.Update(userPreferences);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"User preferences updated for user {userId}");

            return new EnhancedUserPreferencesDto(
                userId,
                userPreferences.Theme,
                userPreferences.Language,
                userPreferences.EmailNotifications,
                userPreferences.PushNotifications,
                userPreferences.TimeZone,
                request.CustomSettings
            );
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating preferences: {ex.Message}");
            throw;
        }
    }
}
