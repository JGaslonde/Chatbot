using Chatbot.API.Data.Repositories;
using Chatbot.Core.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Chatbot.API.Services.Core;

public interface IUserPreferencesService
{
    Task<UserPreferences> GetPreferencesAsync(int userId);
    Task<UserPreferences> UpdatePreferencesAsync(int userId, UserPreferences preferences);
    Task<UserPreferences> CreateDefaultPreferencesAsync(int userId);
}

public class UserPreferencesService : IUserPreferencesService
{
    private readonly Repository<UserPreferences> _preferencesRepository;
    private readonly Repository<User> _userRepository;

    public UserPreferencesService(
        Repository<UserPreferences> preferencesRepository,
        Repository<User> userRepository)
    {
        _preferencesRepository = preferencesRepository;
        _userRepository = userRepository;
    }

    public async Task<UserPreferences> GetPreferencesAsync(int userId)
    {
        var preferences = (await _preferencesRepository.GetAllAsync())
            .FirstOrDefault(p => p.UserId == userId);

        if (preferences == null)
        {
            preferences = await CreateDefaultPreferencesAsync(userId);
        }

        return preferences;
    }

    public async Task<UserPreferences> UpdatePreferencesAsync(int userId, UserPreferences preferences)
    {
        var existing = (await _preferencesRepository.GetAllAsync())
            .FirstOrDefault(p => p.UserId == userId);

        if (existing == null)
        {
            // Create new preferences
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException("User not found");

            preferences.UserId = userId;
            preferences.User = user;
            preferences.CreatedAt = DateTime.UtcNow;
            preferences.UpdatedAt = DateTime.UtcNow;
            await _preferencesRepository.AddAsync(preferences);
            return preferences;
        }

        // Update existing preferences
        existing.Language = preferences.Language;
        existing.Theme = preferences.Theme;
        existing.TimeZone = preferences.TimeZone;
        existing.EmailNotifications = preferences.EmailNotifications;
        existing.PushNotifications = preferences.PushNotifications;
        existing.SoundEnabled = preferences.SoundEnabled;
        existing.ResponseStyle = preferences.ResponseStyle;
        existing.ShowSentimentAnalysis = preferences.ShowSentimentAnalysis;
        existing.ShowIntentRecognition = preferences.ShowIntentRecognition;
        existing.SaveConversationHistory = preferences.SaveConversationHistory;
        existing.AllowDataAnalytics = preferences.AllowDataAnalytics;
        existing.UpdatedAt = DateTime.UtcNow;

        await _preferencesRepository.UpdateAsync(existing);
        return existing;
    }

    public async Task<UserPreferences> CreateDefaultPreferencesAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new InvalidOperationException("User not found");

        var preferences = new UserPreferences
        {
            UserId = userId,
            User = user,
            Language = "en",
            Theme = "light",
            TimeZone = "UTC",
            EmailNotifications = true,
            PushNotifications = true,
            SoundEnabled = true,
            ResponseStyle = "balanced",
            ShowSentimentAnalysis = false,
            ShowIntentRecognition = false,
            SaveConversationHistory = true,
            AllowDataAnalytics = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _preferencesRepository.AddAsync(preferences);
        return preferences;
    }
}
