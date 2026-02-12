using Chatbot.API.Data;
using Chatbot.API.Exceptions;

namespace Chatbot.API.Infrastructure;

/// <summary>
/// Generic service for repository operations with built-in error handling.
/// Applies DRY - Eliminates repeated null-check and exception patterns in services.
/// Implements Dependency Inversion - Services depend on this abstraction.
/// </summary>
public interface IRepositoryService<T> where T : class
{
    Task<T> GetByIdOrThrowAsync(int id, string entityName);
    Task<T?> GetByIdOrDefaultAsync(int id);
}

public class RepositoryService<T> : IRepositoryService<T> where T : class
{
    private readonly IRepository<T> _repository;
    private readonly ILogger<RepositoryService<T>> _logger;

    public RepositoryService(
        IRepository<T> repository,
        ILogger<RepositoryService<T>> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<T> GetByIdOrThrowAsync(int id, string entityName)
    {
        _logger.LogInformation("Retrieving {EntityName} with ID: {Id}", entityName, id);
        
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
        {
            _logger.LogWarning("{EntityName} not found. ID: {Id}", entityName, id);
            throw new NotFoundException(entityName, id);
        }

        return entity;
    }

    public async Task<T?> GetByIdOrDefaultAsync(int id)
    {
        _logger.LogInformation("Attempting to retrieve entity with ID: {Id}", id);
        return await _repository.GetByIdAsync(id);
    }
}
