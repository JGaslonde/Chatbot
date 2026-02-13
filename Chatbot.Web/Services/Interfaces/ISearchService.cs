using Chatbot.Core.Models.Responses;
using Chatbot.Web.Services.Models;

namespace Chatbot.Web.Services.Interfaces;

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
