using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Chatbot.API.Data;

namespace Chatbot.API.Data.Context;

public class ChatbotDbContextFactory : IDesignTimeDbContextFactory<ChatbotDbContext>
{
    public ChatbotDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ChatbotDbContext>();
        var connectionString = "Data Source=chatbot.db";

        optionsBuilder.UseSqlite(connectionString);

        return new ChatbotDbContext(optionsBuilder.Options);
    }
}
