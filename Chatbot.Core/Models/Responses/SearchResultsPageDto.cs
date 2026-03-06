namespace Chatbot.Core.Models.Responses;

/// <summary>Paginated search results</summary>
public record SearchResultsPageDto(
    List<SearchResultDto> Results,
    int PageNumber,
    int PageSize,
    int TotalResults,
    int TotalPages
);
