using Chatbot.Core.Models.Entities;

namespace Chatbot.API.Data.Repositories.Interfaces;

public interface ISearchIndexRepository : IRepository<SearchIndex>
{
    Task<List<SearchIndex>> SearchContentAsync(int userId, string query);
    Task<List<SearchIndex>> SearchByTopicAsync(int userId, string topic);
    Task<List<SearchIndex>> SearchByIntentAsync(int userId, string intent);
    Task<SearchIndex?> GetByConversationIdAsync(int conversationId);
    Task RebuildIndexAsync(int userId);
}
