using Chatbot.API.Data.Repositories.Interfaces;
using Chatbot.API.Infrastructure.Repository.Interfaces;
using Chatbot.API.Exceptions;
using Microsoft.Extensions.Logging;

namespace Chatbot.API.Infrastructure.Repository;

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
