using Chatbot.Core.Models.Requests;
using Chatbot.Core.Models.Responses;

namespace Chatbot.API.Services.Phase3.Interfaces;

public interface ISearchService
{
    Task<SearchResultsPageDto> SearchAsync(int userId, SearchRequest request);
    Task<List<SearchResultDto>> SearchContentAsync(int userId, string query, int skip = 0, int take = 20);
    Task<List<SearchResultDto>> SearchByTopicAsync(int userId, string topic, int skip = 0, int take = 20);
    Task<List<SearchResultDto>> SearchByIntentAsync(int userId, string intent, int skip = 0, int take = 20);
    Task RebuildSearchIndexAsync(int userId);
    Task IndexConversationAsync(int conversationId, int userId, string content, List<string>? topics = null, List<string>? intents = null);
}
