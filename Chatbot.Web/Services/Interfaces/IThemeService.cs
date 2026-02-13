namespace Chatbot.Web.Services.Interfaces;

/// <summary>
/// Theme service for managing dark/light mode
/// </summary>
public interface IThemeService
{
    Task<string> GetCurrentThemeAsync();
    Task SetThemeAsync(string theme);
    void InitializeTheme();
}
