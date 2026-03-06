using Chatbot.Core.Models.Requests;
using Chatbot.Core.Models.Responses;

namespace Chatbot.API.Services.Core.Interfaces;

/// <summary>
/// Service for batch operations on conversations and messages.
/// </summary>
public interface IBatchOperationService
{
    /// <summary>
    /// Performs batch operations on conversations.
    /// </summary>
    Task<BatchOperationResponse> ExecuteBatchOperationAsync(
        int userId,
        BatchOperationRequest request);

    /// <summary>
    /// Deletes multiple conversations.
    /// </summary>
    Task<BatchOperationResponse> DeleteConversationsBatchAsync(int userId, List<int> conversationIds);

    /// <summary>
    /// Archives multiple conversations.
    /// </summary>
    Task<BatchOperationResponse> ArchiveConversationsBatchAsync(int userId, List<int> conversationIds);
}
