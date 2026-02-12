namespace Chatbot.Web.Services;

/// <summary>
/// Theme service for managing dark/light mode
/// </summary>
public interface IThemeService
{
    Task<string> GetCurrentThemeAsync();
    Task SetThemeAsync(string theme);
    void InitializeTheme();
}

public class ThemeService : IThemeService
{
    private const string StorageKey = "chatbot_theme";
    private string _currentTheme = "light";

    public Task<string> GetCurrentThemeAsync()
    {
        return Task.FromResult(_currentTheme);
    }

    public Task SetThemeAsync(string theme)
    {
        _currentTheme = theme;
        // In a real implementation, this would persist to localStorage via JS interop
        return Task.CompletedTask;
    }

    public void InitializeTheme()
    {
        _currentTheme = "light";
        ApplyTheme(_currentTheme);
    }

    private void ApplyTheme(string theme)
    {
        // Theme CSS would be applied here
        // Using document.documentElement.setAttribute('data-theme', theme)
    }
}
