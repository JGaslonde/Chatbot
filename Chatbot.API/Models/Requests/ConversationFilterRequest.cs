namespace Chatbot.API.Models.Requests;

/// <summary>
/// Advanced filtering and pagination for conversation queries
/// </summary>
public class ConversationFilterRequest
{
    /// <summary>
    /// Search text to filter conversations by title or content
    /// </summary>
    public string? SearchText { get; set; }

    /// <summary>
    /// Filter by sentiment range (0.0 to 1.0)
    /// </summary>
    public double? MinSentiment { get; set; }

    /// <summary>
    /// Filter by minimum message count
    /// </summary>
    public int? MinMessages { get; set; }

    /// <summary>
    /// Filter from start date (UTC)
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Filter to end date (UTC)
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Filter by conversation status
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// Filter by tags (comma-separated)
    /// </summary>
    public string? Tags { get; set; }

    /// <summary>
    /// Sort field (created, updated, title, sentiment, messages)
    /// </summary>
    public string SortBy { get; set; } = "created";

    /// <summary>
    /// Sort direction (asc, desc)
    /// </summary>
    public string SortDirection { get; set; } = "desc";

    /// <summary>
    /// Page number (1-based)
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Page size
    /// </summary>
    public int PageSize { get; set; } = 50;
}
