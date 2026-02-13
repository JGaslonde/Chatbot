using Chatbot.API.Exceptions;
using Chatbot.API.Infrastructure.Auth.Interfaces;
using Microsoft.Extensions.Logging;
using Chatbot.API.Infrastructure.Validation.Interfaces;

namespace Chatbot.API.Infrastructure.Validation;

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

        await Task.CompletedTask;
    }

    public async Task EnsureConversationExistsAsync(int conversationId)
    {
        if (conversationId <= 0)
        {
            _logger.LogWarning("Invalid conversation ID: {ConversationId}", conversationId);
            throw new NotFoundException("Conversation", conversationId);
        }

        await Task.CompletedTask;
    }

    public async Task<T> EnsureEntityExistsAsync<T>(int id, Func<int, Task<T?>> getter, string entityName)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Invalid {EntityName} ID: {Id}", entityName, id);
            throw new NotFoundException(entityName, id);
        }

        var entity = await getter(id);
        if (entity == null)
        {
            _logger.LogWarning("{EntityName} not found. ID: {Id}", entityName, id);
            throw new NotFoundException(entityName, id);
        }

        return entity;
    }
}
