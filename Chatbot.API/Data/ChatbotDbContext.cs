using Microsoft.EntityFrameworkCore;
using Chatbot.Core.Models.Entities;

namespace Chatbot.API.Data;

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
    public DbSet<AuditLog> AuditLogs { get; set; } = null!;
    public DbSet<Notification> Notifications { get; set; } = null!;
    public DbSet<UserNotificationPreferences> UserNotificationPreferences { get; set; } = null!;

    // Phase 2 Enterprise Features
    public DbSet<Webhook> Webhooks { get; set; } = null!;
    public DbSet<WebhookDelivery> WebhookDeliveries { get; set; } = null!;
    public DbSet<ApiKey> ApiKeys { get; set; } = null!;
    public DbSet<TwoFactorAuth> TwoFactorAuths { get; set; } = null!;
    public DbSet<IpWhitelist> IpWhitelists { get; set; } = null!;
    public DbSet<ScheduledReport> ScheduledReports { get; set; } = null!;
    public DbSet<ImportJob> ImportJobs { get; set; } = null!;

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

        // Phase 2 Entity Configurations

        // Webhook configuration
        modelBuilder.Entity<Webhook>()
            .HasMany(w => w.Deliveries)
            .WithOne(wd => wd.Webhook)
            .HasForeignKey(wd => wd.WebhookId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Webhook>()
            .HasIndex(w => new { w.UserId, w.IsActive });

        // ApiKey configuration
        modelBuilder.Entity<ApiKey>()
            .HasIndex(ak => ak.KeyHash)
            .IsUnique();

        modelBuilder.Entity<ApiKey>()
            .HasIndex(ak => new { ak.UserId, ak.IsActive });

        // TwoFactorAuth configuration
        modelBuilder.Entity<TwoFactorAuth>()
            .HasIndex(tfa => tfa.UserId)
            .IsUnique();

        // IpWhitelist configuration
        modelBuilder.Entity<IpWhitelist>()
            .HasIndex(iw => new { iw.UserId, iw.IpAddress })
            .IsUnique();

        // ScheduledReport configuration
        modelBuilder.Entity<ScheduledReport>()
            .HasIndex(sr => new { sr.UserId, sr.IsActive });

        // ImportJob configuration
        modelBuilder.Entity<ImportJob>()
            .HasIndex(ij => new { ij.UserId, ij.CreatedAt });

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
