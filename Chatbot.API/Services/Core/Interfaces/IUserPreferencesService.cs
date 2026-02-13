using Chatbot.Core.Models.Entities;

namespace Chatbot.API.Services.Core.Interfaces;

public interface IUserPreferencesService
{
    Task<UserPreferences> GetPreferencesAsync(int userId);
    Task<UserPreferences> UpdatePreferencesAsync(int userId, UserPreferences preferences);
    Task<UserPreferences> CreateDefaultPreferencesAsync(int userId);
}
