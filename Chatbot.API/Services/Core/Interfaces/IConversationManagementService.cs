using Chatbot.API.Models.Requests;
using Chatbot.API.Models.Responses;

namespace Chatbot.API.Services.Core.Interfaces;

/// <summary>
/// Advanced conversation management with filtering, pagination, and bulk operations
/// </summary>
public interface IConversationManagementService
{
    /// <summary>
    /// Get paginated conversation list with advanced filtering
    /// </summary>
    Task<PaginatedResponse<ConversationSummaryDto>> GetConversationsAsync(
        int userId,
        ConversationFilterRequest filter);

    /// <summary>
    /// Get detailed conversation information for viewing
    /// </summary>
    Task<object?> GetConversationDetailAsync(int conversationId, int userId);

    /// <summary>
    /// Archive a conversation
    /// </summary>
    Task<bool> ArchiveConversationAsync(int conversationId, int userId);

    /// <summary>
    /// Restore an archived conversation
    /// </summary>
    Task<bool> RestoreConversationAsync(int conversationId, int userId);

    /// <summary>
    /// Soft delete conversation (mark as deleted)
    /// </summary>
    Task<bool> DeleteConversationAsync(int conversationId, int userId);

    /// <summary>
    /// Permanently delete conversation (cannot be recovered)
    /// </summary>
    Task<bool> PermanentlyDeleteConversationAsync(int conversationId, int userId);

    /// <summary>
    /// Add tags to conversation
    /// </summary>
    Task<bool> UpdateConversationTagsAsync(int conversationId, List<string> tags, int userId);

    /// <summary>
    /// Update conversation status
    /// </summary>
    Task<bool> UpdateConversationStatusAsync(int conversationId, string status, int userId);

    /// <summary>
    /// Assign conversation to category
    /// </summary>
    Task<bool> UpdateConversationCategoryAsync(int conversationId, string category, int userId);

    /// <summary>
    /// Perform bulk operations on multiple conversations
    /// </summary>
    Task<BulkOperationResultDto> PerformBulkOperationAsync(
        BulkConversationOperationRequest request,
        int userId);

    /// <summary>
    /// Get conversation count by status
    /// </summary>
    Task<Dictionary<string, int>> GetConversationCountByStatusAsync(int userId);

    /// <summary>
    /// Search conversations by text and metadata
    /// </summary>
    Task<List<ConversationSummaryDto>> SearchConversationsAsync(
        int userId,
        string searchText);
}
