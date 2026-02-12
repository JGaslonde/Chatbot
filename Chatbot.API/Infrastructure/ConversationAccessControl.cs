using Chatbot.API.Exceptions;
using Chatbot.API.Services;

namespace Chatbot.API.Infrastructure;

/// <summary>
/// Handles conversation access control and authorization checks.
/// Applies DRY - Centralizes repeated authorization logic.
/// Single Responsibility - Focuses solely on authorization concerns.
/// </summary>
public interface IConversationAccessControl
{
    Task<bool> HasAccessAsync(int conversationId, int userId);
    Task VerifyAccessAsync(int conversationId, int userId);
}

public class ConversationAccessControl : IConversationAccessControl
{
    private readonly IConversationService _conversationService;
    private readonly ILogger<ConversationAccessControl> _logger;

    public ConversationAccessControl(
        IConversationService conversationService,
        ILogger<ConversationAccessControl> logger)
    {
        _conversationService = conversationService;
        _logger = logger;
    }

    public async Task<bool> HasAccessAsync(int conversationId, int userId)
    {
        try
        {
            var conversation = await _conversationService.GetConversationAsync(conversationId);
            return conversation?.UserId == userId;
        }
        catch
        {
            _logger.LogWarning("Access check failed for conversation {ConversationId} by user {UserId}", conversationId, userId);
            return false;
        }
    }

    public async Task VerifyAccessAsync(int conversationId, int userId)
    {
        var hasAccess = await HasAccessAsync(conversationId, userId);
        if (!hasAccess)
        {
            _logger.LogWarning("Access denied to conversation {ConversationId} for user {UserId}", conversationId, userId);
            throw new UnauthorizedException("Access denied to this conversation");
        }
    }
}
