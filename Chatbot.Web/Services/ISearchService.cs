namespace Chatbot.Web.Services;

using Chatbot.Core.Models;

/// <summary>
/// Service interface for searching and filtering conversations and messages.
/// </summary>
public interface ISearchService
{
    /// <summary>
    /// Searches conversations by title and summary.
    /// </summary>
    List<ConversationResponse> SearchConversations(List<ConversationResponse> conversations, string query);

    /// <summary>
    /// Searches messages by content.
    /// </summary>
    List<MessageDto> SearchMessages(List<MessageDto> messages, string query);

    /// <summary>
    /// Gets conversation statistics for the given query.
    /// </summary>
    ConversationStats GetStats(List<ConversationResponse> conversations);
}

/// <summary>
/// Statistics for a set of conversations.
/// </summary>
public class ConversationStats
{
    public int TotalConversations { get; set; }
    public int TotalMessages { get; set; }
}
