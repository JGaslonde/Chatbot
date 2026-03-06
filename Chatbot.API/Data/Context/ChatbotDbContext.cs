using Microsoft.EntityFrameworkCore;
using Chatbot.Core.Models.Entities;

namespace Chatbot.API.Data.Context;

public class ChatbotDbContext : DbContext
{
    public ChatbotDbContext(DbContextOptions<ChatbotDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Conversation> Conversations { get; set; } = null!;
    public DbSet<Message> Messages { get; set; } = null!;
    public DbSet<UserPreferences> UserPreferences { get; set; } = null!;
    public DbSet<MessageFeedback> MessageFeedback { get; set; } = null!;
    public DbSet<EscalationTicket> EscalationTickets { get; set; } = null!;
    public DbSet<KnowledgeEntry> KnowledgeEntries { get; set; } = null!;
    public DbSet<RevokedToken> RevokedTokens { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasMany(u => u.Conversations)
            .WithOne(c => c.User)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // UserPreferences configuration
        modelBuilder.Entity<UserPreferences>()
            .HasOne(p => p.User)
            .WithOne(u => u.Preferences)
            .HasForeignKey<UserPreferences>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Conversation configuration
        modelBuilder.Entity<Conversation>()
            .HasMany(c => c.Messages)
            .WithOne(m => m.Conversation)
            .HasForeignKey(m => m.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Conversation>()
            .HasIndex(c => new { c.UserId, c.IsActive });

        // Message configuration
        modelBuilder.Entity<Message>()
            .HasIndex(m => new { m.ConversationId, m.SentAt });

        // MessageFeedback configuration
        modelBuilder.Entity<MessageFeedback>()
            .HasOne(f => f.Message)
            .WithMany(m => m.Feedback)
            .HasForeignKey(f => f.MessageId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MessageFeedback>()
            .HasOne(f => f.User)
            .WithMany()
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MessageFeedback>()
            .HasIndex(f => new { f.MessageId, f.UserId })
            .IsUnique();

        // EscalationTicket configuration
        modelBuilder.Entity<EscalationTicket>()
            .HasOne(t => t.Conversation)
            .WithMany()
            .HasForeignKey(t => t.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<EscalationTicket>()
            .HasOne(t => t.User)
            .WithMany()
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<EscalationTicket>()
            .HasIndex(t => t.Status);

        // KnowledgeEntry configuration
        modelBuilder.Entity<KnowledgeEntry>()
            .HasIndex(k => k.Category);

        // RevokedToken configuration
        modelBuilder.Entity<RevokedToken>()
            .HasIndex(r => r.Jti)
            .IsUnique();

        modelBuilder.Entity<RevokedToken>()
            .HasIndex(r => r.ExpiresAt);

        // Seed initial data if needed
        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        // Seed a test user (password hash would be generated at runtime)
        var testUser = new User
        {
            Id = 1,
            Username = "testuser",
            Email = "test@chatbot.local",
            PasswordHash = "$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcg7b3XeKeUxWdeS86AGR57XO1i", // bcrypt hash of "password123"
            DisplayName = "Test User",
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        modelBuilder.Entity<User>().HasData(testUser);
    }
}
