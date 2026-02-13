using Chatbot.Core.Models.Requests;
using Chatbot.Core.Models.Responses;

namespace Chatbot.API.Services.Analysis.Interfaces;

/// <summary>
/// Service for advanced search and filtering of conversations with pagination and sorting.
/// </summary>
public interface IAdvancedSearchService
{
    /// <summary>
    /// Searches conversations with advanced filtering, sorting, and pagination.
    /// </summary>
    Task<SearchResultsResponse> SearchConversationsAsync(
        int userId,
        SearchConversationsRequest request);

    /// <summary>
    /// Searches messages within conversations with filtering and pagination.
    /// </summary>
    Task<SearchResultsResponse> SearchMessagesAsync(
        int userId,
        string query,
        int page = 1,
        int pageSize = 20);
}
