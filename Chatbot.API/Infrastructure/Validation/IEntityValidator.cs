using Chatbot.Core.Models.Entities;

namespace Chatbot.API.Infrastructure.Validation;

public interface IEntityValidator
{
    Task EnsureUserExistsAsync(int userId);
    Task EnsureConversationExistsAsync(int conversationId);
    Task<T> EnsureEntityExistsAsync<T>(int id, Func<int, Task<T?>> getter, string entityName);
}
