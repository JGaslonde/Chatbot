namespace Chatbot.Core.Models.Requests;

/// <summary>Request to search conversations</summary>
public record SearchRequest(
    string Query,
    string? SearchType = "all", // all, content, topic, intent, keyword
    DateTime? DateFrom = null,
    DateTime? DateTo = null,
    int PageNumber = 1,
    int PageSize = 20,
    string? SortBy = "relevance" // relevance, date, sentiment
);
