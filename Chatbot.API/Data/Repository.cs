using Chatbot.API.Models.Entities;

namespace Chatbot.API.Data;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task<bool> DeleteAsync(int id);
    Task<int> SaveChangesAsync();
}

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly ChatbotDbContext _context;

    public Repository(ChatbotDbContext context)
    {
        _context = context;
    }

    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await _context.Set<T>().FindAsync(id);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await Task.FromResult(_context.Set<T>().ToList());
    }

    public virtual async Task<T> AddAsync(T entity)
    {
        _context.Set<T>().Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public virtual async Task<T> UpdateAsync(T entity)
    {
        _context.Set<T>().Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public virtual async Task<bool> DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity == null)
            return false;

        _context.Set<T>().Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public virtual async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByEmailAsync(string email);
}

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(ChatbotDbContext context) : base(context) { }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await Task.FromResult(_context.Users.FirstOrDefault(u => u.Username == username));
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await Task.FromResult(_context.Users.FirstOrDefault(u => u.Email == email));
    }
}

public interface IConversationRepository : IRepository<Conversation>
{
    Task<IEnumerable<Conversation>> GetUserConversationsAsync(int userId);
    Task<Conversation?> GetWithMessagesAsync(int id);
}

public class ConversationRepository : Repository<Conversation>, IConversationRepository
{
    public ConversationRepository(ChatbotDbContext context) : base(context) { }

    public async Task<IEnumerable<Conversation>> GetUserConversationsAsync(int userId)
    {
        return await Task.FromResult(
            _context.Conversations
                .Where(c => c.UserId == userId && c.IsActive)
                .OrderByDescending(c => c.LastMessageAt)
                .ToList()
        );
    }

    public async Task<Conversation?> GetWithMessagesAsync(int id)
    {
        return await Task.FromResult(
            _context.Conversations
                .Where(c => c.Id == id)
                .Include(c => c.Messages)
                .FirstOrDefault()
        );
    }
}

public interface IMessageRepository : IRepository<Message>
{
    Task<IEnumerable<Message>> GetConversationMessagesAsync(int conversationId);
}

public class MessageRepository : Repository<Message>, IMessageRepository
{
    public MessageRepository(ChatbotDbContext context) : base(context) { }

    public async Task<IEnumerable<Message>> GetConversationMessagesAsync(int conversationId)
    {
        return await Task.FromResult(
            _context.Messages
                .Where(m => m.ConversationId == conversationId)
                .OrderBy(m => m.SentAt)
                .ToList()
        );
    }
}
