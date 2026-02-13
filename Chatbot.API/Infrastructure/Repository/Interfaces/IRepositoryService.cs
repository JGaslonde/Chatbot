namespace Chatbot.API.Infrastructure.Repository.Interfaces;

public interface IRepositoryService<T> where T : class
{
    Task<T> GetByIdOrThrowAsync(int id, string entityName);
    Task<T?> GetByIdOrDefaultAsync(int id);
}
