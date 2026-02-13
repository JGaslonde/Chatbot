namespace Chatbot.Core.Models.Requests;

/// <summary>
/// Request for advanced conversation search with filtering options.
/// </summary>
public record SearchConversationsRequest(
    string? Query = null,
    DateTime? StartDate = null,
    DateTime? EndDate = null,
    string? SortBy = "lastMessageAt",
    bool AscendingOrder = false,
    int Page = 1,
    int PageSize = 20
);
