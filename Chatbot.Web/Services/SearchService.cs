namespace Chatbot.Web.Services;

using Chatbot.Core.Models;

/// <summary>
/// Service for searching and filtering conversations and messages.
/// </summary>
public class SearchService : ISearchService
{
    /// <summary>
    /// Searches conversations by title and summary using case-insensitive matching.
    /// </summary>
    public List<ConversationResponse> SearchConversations(List<ConversationResponse> conversations, string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return conversations;

        var lowerQuery = query.ToLower();
        return conversations
            .Where(c =>
                (c.Title?.ToLower().Contains(lowerQuery) ?? false) ||
                (c.Summary?.ToLower().Contains(lowerQuery) ?? false)
            )
            .ToList();
    }

    /// <summary>
    /// Searches messages by content using case-insensitive matching.
    /// </summary>
    public List<MessageDto> SearchMessages(List<MessageDto> messages, string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return messages;

        var lowerQuery = query.ToLower();
        return messages
            .Where(m => m.Content.ToLower().Contains(lowerQuery))
            .ToList();
    }

    /// <summary>
    /// Gets statistics for a set of conversations.
    /// </summary>
    public ConversationStats GetStats(List<ConversationResponse> conversations)
    {
        var stats = new ConversationStats
        {
            TotalConversations = conversations.Count,
            TotalMessages = conversations.Sum(c => c.MessageCount),
        };

        return stats;
    }
}
