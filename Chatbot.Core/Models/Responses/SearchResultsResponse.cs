namespace Chatbot.Core.Models.Responses;

/// <summary>
/// Paginated search results response.
/// </summary>
public record SearchResultsResponse(
    List<ConversationResponse> Results,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages
);
