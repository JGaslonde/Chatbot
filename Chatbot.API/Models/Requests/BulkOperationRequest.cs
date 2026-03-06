namespace Chatbot.API.Models.Requests;

/// <summary>
/// Bulk operation request for batch processing conversations
/// </summary>
public class BulkConversationOperationRequest
{
    /// <summary>
    /// IDs of conversations to operate on
    /// </summary>
    public List<int> ConversationIds { get; set; } = new();

    /// <summary>
    /// Operation type (tag, archive, delete, status-update, export)
    /// </summary>
    public string OperationType { get; set; } = string.Empty;

    /// <summary>
    /// New tags to add
    /// </summary>
    public List<string>? TagsToAdd { get; set; }

    /// <summary>
    /// Tags to remove
    /// </summary>
    public List<string>? TagsToRemove { get; set; }

    /// <summary>
    /// New status value
    /// </summary>
    public string? NewStatus { get; set; }

    /// <summary>
    /// Category to assign
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// Flag for archiving
    /// </summary>
    public bool Archive { get; set; }

    /// <summary>
    /// Delete permanently (dangerous)
    /// </summary>
    public bool PermanentlyDelete { get; set; }
}

/// <summary>
/// Request for exporting conversations
/// </summary>
public class ExportConversationsRequest
{
    /// <summary>
    /// Format to export (json, csv, pdf)
    /// </summary>
    public string Format { get; set; } = "json";

    /// <summary>
    /// Conversation IDs to export
    /// </summary>
    public List<int>? ConversationIds { get; set; }

    /// <summary>
    /// Export all matching filter
    /// </summary>
    public ConversationFilterRequest? FilterCriteria { get; set; }

    /// <summary>
    /// Include metadata
    /// </summary>
    public bool IncludeMetadata { get; set; } = true;

    /// <summary>
    /// Include embeddings/vectors
    /// </summary>
    public bool IncludeEmbeddings { get; set; } = false;

    /// <summary>
    /// Filename for exported file
    /// </summary>
    public string FileName { get; set; } = "export";
}
