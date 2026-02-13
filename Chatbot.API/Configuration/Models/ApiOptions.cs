namespace Chatbot.API.Configuration.Models;

/// <summary>
/// Configuration options for API behavior.
/// </summary>
public class ApiOptions
{
    public const string SectionName = "Api";

    /// <summary>
    /// Default page size for paginated results.
    /// </summary>
    public int DefaultPageSize { get; set; } = 10;

    /// <summary>
    /// Maximum page size for paginated results.
    /// </summary>
    public int MaxPageSize { get; set; } = 100;

    /// <summary>
    /// JWT token expiration in minutes.
    /// </summary>
    public int TokenExpirationMinutes { get; set; } = 1440;

    /// <summary>
    /// Enable detailed logging.
    /// </summary>
    public bool EnableDetailedLogging { get; set; } = false;
}
