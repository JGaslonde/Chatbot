using Chatbot.Core.Models.Requests;
using Chatbot.Core.Models.Responses;
using Chatbot.API.Data.Repositories.Interfaces;
using Chatbot.API.Services.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Chatbot.API.Services.Core;

public class BatchOperationService : IBatchOperationService
{
    private readonly IConversationRepository _conversationRepository;
    private readonly IAuditLoggingService _auditLogging;
    private readonly ILogger<BatchOperationService> _logger;

    public BatchOperationService(
        IConversationRepository conversationRepository,
        IAuditLoggingService auditLogging,
        ILogger<BatchOperationService> logger)
    {
        _conversationRepository = conversationRepository;
        _auditLogging = auditLogging;
        _logger = logger;
    }

    public async Task<BatchOperationResponse> ExecuteBatchOperationAsync(
        int userId,
        BatchOperationRequest request)
    {
        try
        {
            return request.Operation.ToLower() switch
            {
                "delete" => await DeleteConversationsBatchAsync(userId, request.ConversationIds),
                "archive" => await ArchiveConversationsBatchAsync(userId, request.ConversationIds),
                _ => new BatchOperationResponse(false, "Unknown operation", 0, request.ConversationIds.Count,
                    new List<string> { $"Operation '{request.Operation}' is not supported" })
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing batch operation for user {UserId}", userId);
            throw;
        }
    }

    public async Task<BatchOperationResponse> DeleteConversationsBatchAsync(int userId, List<int> conversationIds)
    {
        var processed = 0;
        var failed = 0;
        var errors = new List<string>();

        foreach (var conversationId in conversationIds)
        {
            try
            {
                var conversation = await _conversationRepository.GetByIdAsync(conversationId);

                if (conversation == null)
                {
                    errors.Add($"Conversation {conversationId} not found");
                    failed++;
                    continue;
                }

                if (conversation.UserId != userId)
                {
                    errors.Add($"Unauthorized to delete conversation {conversationId}");
                    failed++;
                    continue;
                }

                await _conversationRepository.DeleteAsync(conversationId);
                await _auditLogging.LogActionAsync(userId, "Delete", "Conversation", conversationId);
                processed++;
            }
            catch (Exception ex)
            {
                errors.Add($"Error deleting conversation {conversationId}: {ex.Message}");
                failed++;
                _logger.LogError(ex, "Error deleting conversation {ConversationId}", conversationId);
            }
        }

        return new BatchOperationResponse(
            failed == 0,
            $"Batch delete completed: {processed} succeeded, {failed} failed",
            processed,
            failed,
            errors
        );
    }

    public async Task<BatchOperationResponse> ArchiveConversationsBatchAsync(int userId, List<int> conversationIds)
    {
        var processed = 0;
        var failed = 0;
        var errors = new List<string>();

        foreach (var conversationId in conversationIds)
        {
            try
            {
                var conversation = await _conversationRepository.GetByIdAsync(conversationId);

                if (conversation == null)
                {
                    errors.Add($"Conversation {conversationId} not found");
                    failed++;
                    continue;
                }

                if (conversation.UserId != userId)
                {
                    errors.Add($"Unauthorized to archive conversation {conversationId}");
                    failed++;
                    continue;
                }

                conversation.IsActive = false;
                await _conversationRepository.UpdateAsync(conversation);
                await _auditLogging.LogActionAsync(userId, "Archive", "Conversation", conversationId);
                processed++;
            }
            catch (Exception ex)
            {
                errors.Add($"Error archiving conversation {conversationId}: {ex.Message}");
                failed++;
                _logger.LogError(ex, "Error archiving conversation {ConversationId}", conversationId);
            }
        }

        return new BatchOperationResponse(
            failed == 0,
            $"Batch archive completed: {processed} succeeded, {failed} failed",
            processed,
            failed,
            errors
        );
    }
}
