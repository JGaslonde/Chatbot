using Chatbot.API.Exceptions;
using Chatbot.API.Infrastructure.Auth;
using Microsoft.Extensions.Logging;

namespace Chatbot.API.Infrastructure.Validation;

/// <summary>
/// Provides utility methods for common validation operations.
/// Applies DRY - Consolidates repeated validation checks throughout services.
/// </summary>
public interface IEntityValidator
{
    Task EnsureUserExistsAsync(int userId);
    Task EnsureConversationExistsAsync(int conversationId);
    Task<T> EnsureEntityExistsAsync<T>(int id, Func<int, Task<T?>> getter, string entityName);
}

public class EntityValidator : IEntityValidator
{
    private readonly IUserContextProvider _userContext;
    private readonly ILogger<EntityValidator> _logger;

    public EntityValidator(
        IUserContextProvider userContext,
        ILogger<EntityValidator> logger)
    {
        _userContext = userContext;
        _logger = logger;
    }

    public async Task EnsureUserExistsAsync(int userId)
    {
        // This is a simple check - can be enhanced with actual repository call
        if (userId <= 0)
        {
            _logger.LogWarning("Invalid user ID: {UserId}", userId);
            throw new UnauthorizedException("Invalid user");
        }
    }

    public async Task EnsureConversationExistsAsync(int conversationId)
    {
        if (conversationId <= 0)
        {
            _logger.LogWarning("Invalid conversation ID: {ConversationId}", conversationId);
            throw new NotFoundException("Conversation", conversationId);
        }
    }

    public async Task<T> EnsureEntityExistsAsync<T>(int id, Func<int, Task<T?>> getter, string entityName)
    {
        var entity = await getter(id);
        if (entity == null)
        {
            _logger.LogWarning("Entity not found. Type: {EntityName}, ID: {Id}", entityName, id);
            throw new NotFoundException(entityName, id);
        }

        return entity;
    }
}
