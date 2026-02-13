namespace Chatbot.Web.Services.Interfaces;

public interface IPreferencesService
{
    Task<(bool Success, string Message, UserPreferences? Preferences)> GetPreferencesAsync();
    Task<(bool Success, string Message, UserPreferences? Preferences)> UpdatePreferencesAsync(UserPreferences preferences);
}

