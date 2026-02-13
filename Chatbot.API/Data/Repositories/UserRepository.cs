using Chatbot.API.Data.Context;
using Chatbot.Core.Models.Entities;
using Chatbot.API.Data.Repositories.Interfaces;

namespace Chatbot.API.Data.Repositories;

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
