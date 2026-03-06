namespace Chatbot.API.Models.Responses;

/// <summary>
/// Paginated response wrapper for conversation results
/// </summary>
public class PaginatedResponse<T>
{
    /// <summary>
    /// Current page of results
    /// </summary>
    public List<T> Items { get; set; } = new();

    /// <summary>
    /// Current page number
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// Items per page
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total number of items
    /// </summary>
    public int TotalItems { get; set; }

    /// <summary>
    /// Total number of pages
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// Whether more pages exist
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;

    /// <summary>
    /// Whether previous pages exist
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;
}

/// <summary>
/// Conversation summary for list operations
/// </summary>
public class ConversationSummaryDto
{
    /// <summary>
    /// Conversation ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Conversation title
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Message count
    /// </summary>
    public int MessageCount { get; set; }

    /// <summary>
    /// Sentiment score (0-1)
    /// </summary>
    public double SentimentScore { get; set; }

    /// <summary>
    /// Creation date
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Last update date
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Last message date
    /// </summary>
    public DateTime? LastMessageAt { get; set; }

    /// <summary>
    /// Conversation status
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// Associated tags
    /// </summary>
    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// Category assignment
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// Is archived
    /// </summary>
    public bool IsArchived { get; set; }
}
