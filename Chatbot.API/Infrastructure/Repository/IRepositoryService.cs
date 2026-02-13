using Chatbot.API.Data.Repositories;

namespace Chatbot.API.Infrastructure.Repository;

public interface IRepositoryService<T> where T : class
{
    Task<T> GetByIdOrThrowAsync(int id, string entityName);
    Task<T?> GetByIdOrDefaultAsync(int id);
}
