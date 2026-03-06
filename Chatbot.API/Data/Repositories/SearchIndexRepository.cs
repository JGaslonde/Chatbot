using Chatbot.API.Data.Repositories.Interfaces;
using Chatbot.Core.Models.Entities;

namespace Chatbot.API.Data.Repositories;

public class SearchIndexRepository : Repository<SearchIndex>, ISearchIndexRepository
{
    public SearchIndexRepository(ChatbotDbContext context) : base(context) { }

    public async Task<List<SearchIndex>> SearchContentAsync(int userId, string query)
    {
        var lowerQuery = query.ToLower();
        return _context.Set<SearchIndex>().Where(x => x.UserId == userId && x.Content.ToLower().Contains(lowerQuery))
                     .OrderByDescending(x => x.RelevanceScore)
                     .ToList();
    }

    public async Task<List<SearchIndex>> SearchByTopicAsync(int userId, string topic) =>
        _context.Set<SearchIndex>().Where(x => x.UserId == userId && x.Topics != null && x.Topics.Contains(topic))
              .OrderByDescending(x => x.IndexedAt)
              .ToList();

    public async Task<List<SearchIndex>> SearchByIntentAsync(int userId, string intent) =>
        _context.Set<SearchIndex>().Where(x => x.UserId == userId && x.Intents != null && x.Intents.Contains(intent))
              .OrderByDescending(x => x.IndexedAt)
              .ToList();

    public async Task<SearchIndex?> GetByConversationIdAsync(int conversationId) =>
        _context.Set<SearchIndex>().FirstOrDefault(x => x.ConversationId == conversationId);

    public async Task RebuildIndexAsync(int userId)
    {
        // In a real implementation, this would rebuild the search index from conversation data
        var existingIndexes = _context.Set<SearchIndex>().Where(x => x.UserId == userId).ToList();
        foreach (var index in existingIndexes)
        {
            index.IndexedAt = DateTime.UtcNow;
        }
    }
}
