using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Chatbot.API.Data;
using Chatbot.API.Data.Repositories;
using Chatbot.API.Data.Repositories.Interfaces;
using Chatbot.API.Services.Core.Interfaces;
using Chatbot.API.Services.Analysis.Interfaces;
using Chatbot.API.Services.Processing.Interfaces;
using Chatbot.API.Services.Export.Interfaces;
using Chatbot.API.Services.Analytics.Interfaces;
using Chatbot.API.Services.Core;
using Chatbot.API.Services.Analysis;
using Chatbot.API.Services.Processing;
using Chatbot.API.Services.Export;
using Chatbot.API.Services.Analytics;
using Chatbot.API.Middleware;
using Chatbot.API.Hubs;
using Chatbot.API.Infrastructure.Auth.Interfaces;
using Chatbot.API.Infrastructure.Auth;
using Chatbot.API.Infrastructure.Authorization.Interfaces;
using Chatbot.API.Infrastructure.Authorization;
using Chatbot.API.Infrastructure.Http.Interfaces;
using Chatbot.API.Infrastructure.Http;
using Chatbot.API.Infrastructure.Facades.Interfaces;
using Chatbot.API.Infrastructure.Facades;
using Chatbot.API.Infrastructure.Repository;
using Chatbot.Core.Models.Entities;
using FluentValidation;
using FluentValidation.AspNetCore;
using Chatbot.API.Validators;
using Chatbot.API.Services.Admin;
using Chatbot.API.Services.Admin.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add database context
// Use InMemory database for testing if environment variable is set
var useInMemoryDatabase = builder.Environment.EnvironmentName == "Testing";
if (useInMemoryDatabase)
{
    // Will be overridden by test configuration
}
else
{
    builder.Services.AddDbContext<ChatbotDbContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ??
        "Data Source=chatbot.db"));
}

// Add services to the container
builder.Services.AddControllers();

// Add FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<ChatMessageRequestValidator>();

// Add JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured");
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };

    // Configure JWT authentication for SignalR
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;

            // If the request is for our hub and has a token, use it
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/chat"))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

builder.Services.AddSwaggerGen();

// Add SignalR
builder.Services.AddSignalR();

// Register repositories (wrapped for design-time compatibility with EF Core migrations)
try
{
    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<IConversationRepository, ConversationRepository>();
    builder.Services.AddScoped<IMessageRepository, MessageRepository>();
    builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
    builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
    builder.Services.AddScoped<IUserNotificationPreferencesRepository, UserNotificationPreferencesRepository>();
    // Phase 2 Enterprise Features repositories
    builder.Services.AddScoped<IWebhookRepository, WebhookRepository>();
    builder.Services.AddScoped<IWebhookDeliveryRepository, WebhookDeliveryRepository>();
    builder.Services.AddScoped<IApiKeyRepository, ApiKeyRepository>();
    builder.Services.AddScoped<ITwoFactorAuthRepository, TwoFactorAuthRepository>();
    builder.Services.AddScoped<IIpWhitelistRepository, IpWhitelistRepository>();
    builder.Services.AddScoped<IScheduledReportRepository, ScheduledReportRepository>();
    builder.Services.AddScoped<IImportJobRepository, ImportJobRepository>();
    // Register generic repositories for new entities
    builder.Services.AddScoped<Repository<Message>>();
    builder.Services.AddScoped<Repository<User>>();
    builder.Services.AddScoped<Repository<Conversation>>();
    builder.Services.AddScoped<Repository<UserPreferences>>();
    builder.Services.AddScoped<Repository<AuditLog>>();
    builder.Services.AddScoped<Repository<Notification>>();
    builder.Services.AddScoped<Repository<UserNotificationPreferences>>();
    // Phase 2 generic repositories
    builder.Services.AddScoped<Repository<Webhook>>();
    builder.Services.AddScoped<Repository<WebhookDelivery>>();
    builder.Services.AddScoped<Repository<ApiKey>>();
    builder.Services.AddScoped<Repository<TwoFactorAuth>>();
    builder.Services.AddScoped<Repository<IpWhitelist>>();
    builder.Services.AddScoped<Repository<ScheduledReport>>();
    builder.Services.AddScoped<Repository<ImportJob>>();
}
catch (Exception ex)
{
    // Design-time context resolution may fail during migration generation
    // This is safe to suppress as migrations can still be generated with DbContextFactory
    Console.WriteLine($"Note: Some repositories skipped during startup: {ex.Message}");
}
// Phase 3 Advanced Features repositories (wrapped for design-time compatibility)
try
{
    builder.Services.AddScoped<IConversationAnalyticsRepository, ConversationAnalyticsRepository>();
    builder.Services.AddScoped<IMLInsightRepository, MLInsightRepository>();
    builder.Services.AddScoped<IWorkflowRepository, WorkflowRepository>();
    builder.Services.AddScoped<IUserSegmentRepository, UserSegmentRepository>();
    builder.Services.AddScoped<ISearchIndexRepository, SearchIndexRepository>();
    // Phase 3 generic repositories
    builder.Services.AddScoped<Repository<ConversationAnalyticsEntity>>();
    builder.Services.AddScoped<Repository<MLInsight>>();
    builder.Services.AddScoped<Repository<WorkflowDefinition>>();
    builder.Services.AddScoped<Repository<WorkflowExecution>>();
    builder.Services.AddScoped<Repository<UserSegment>>();
    builder.Services.AddScoped<Repository<SearchIndex>>();

    // Register generic repository interfaces needed by services
    builder.Services.AddScoped<IRepository<Message>, Repository<Message>>();
    builder.Services.AddScoped<IRepository<WorkflowExecution>, Repository<WorkflowExecution>>();
    builder.Services.AddScoped<IRepository<User>, Repository<User>>();
    builder.Services.AddScoped<IRepository<Conversation>, Repository<Conversation>>();
}
catch (Exception ex)
{
    // Design-time context resolution may fail during migration generation
    // This is safe to suppress as migrations can still be generated
    Console.WriteLine($"Note: Phase 3 repositories skipped during startup: {ex.Message}");
}

// Register infrastructure services (wrapped for design-time compatibility)
try
{
    // IHttpContextAccessor is already available - don't register explicitly
    builder.Services.AddScoped<IUserContextProvider, UserContextProvider>();
    builder.Services.AddScoped<IApiResponseBuilder, ApiResponseBuilder>();
    builder.Services.AddScoped<IConversationAccessControl, ConversationAccessControl>();
    builder.Services.AddScoped<IChatFacadeService, ChatFacadeService>();
}
catch (Exception ex)
{
    Console.WriteLine($"Note: Infrastructure services skipped during startup: {ex.Message}");
}

// Register application services (wrapped for design-time compatibility)
try
{
    builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
    builder.Services.AddScoped<IMessageAnalyticsService, MessageAnalyticsService>();
    builder.Services.AddScoped<IConversationService, ConversationService>();
    builder.Services.AddScoped<ISentimentAnalysisService, SimpleSentimentAnalysisService>();
    builder.Services.AddScoped<IIntentRecognitionService, SimpleIntentRecognitionService>();
    builder.Services.AddScoped<IMessageFilterService, MessageFilterService>();
    builder.Services.AddScoped<IResponseTemplateService, ResponseTemplateService>();
    builder.Services.AddScoped<IConversationSummarizationService, ConversationSummarizationService>();
    builder.Services.AddScoped<IConversationAnalyticsService, ConversationAnalyticsService>();
    builder.Services.AddScoped<IUserPreferencesService, UserPreferencesService>();
    builder.Services.AddScoped<IConversationExportService, ConversationExportService>();
    // Register Admin services
    builder.Services.AddScoped<IAdminService, AdminService>();

    // Register Advanced API services
    builder.Services.AddScoped<IConversationManagementService, ConversationManagementService>();
    builder.Services.AddScoped<IAnalyticsReportingService, AnalyticsReportingService>();
    builder.Services.AddScoped<IActivityTrackingService, ActivityTrackingService>();
    builder.Services.AddScoped<ISystemMetricsService, SystemMetricsService>();
    builder.Services.AddScoped<IAdvancedDataExportService, AdvancedDataExportService>();
    builder.Services.AddScoped<IAuditService, AuditService>();
    // Register Search services
    builder.Services.AddScoped<SavedSearchService>();
    // Register new expansion services
    builder.Services.AddScoped<IAdvancedSearchService, AdvancedSearchService>();
    builder.Services.AddScoped<IAdvancedAnalyticsService, AdvancedAnalyticsService>();
    builder.Services.AddScoped<IAuditLoggingService, AuditLoggingService>();
    builder.Services.AddScoped<IExportService, ExportService>();
    builder.Services.AddScoped<IBatchOperationService, BatchOperationService>();
    builder.Services.AddScoped<INotificationService, NotificationService>();
    // Phase 2 Enterprise Features services
    builder.Services.AddScoped<IWebhookService, WebhookService>();
    builder.Services.AddScoped<IApiKeyService, ApiKeyService>();
    builder.Services.AddScoped<ITwoFactorService, TwoFactorService>();
    builder.Services.AddScoped<IIpWhitelistService, IpWhitelistService>();
    builder.Services.AddScoped<IReportingService, ReportingService>();
    builder.Services.AddScoped<IImportService, ImportService>();
    builder.Services.AddScoped<ICacheService, CacheService>();
    builder.Services.AddScoped<IUserPreferencesEnhancedService, UserPreferencesEnhancedService>();
}
catch (Exception ex)
{
    Console.WriteLine($"Note: Application services skipped during startup: {ex.Message}");
}

// Phase 3 Advanced Features services (wrapped for design-time compatibility)
try
{
    builder.Services.AddScoped<Chatbot.API.Services.Phase3.Interfaces.IConversationAnalyticsService, Chatbot.API.Services.Phase3.ConversationAnalyticsService>();
    builder.Services.AddScoped<Chatbot.API.Services.Phase3.Interfaces.IMLInsightService, Chatbot.API.Services.Phase3.MLInsightService>();
    builder.Services.AddScoped<Chatbot.API.Services.Phase3.Interfaces.IWorkflowService, Chatbot.API.Services.Phase3.WorkflowService>();
    builder.Services.AddScoped<Chatbot.API.Services.Phase3.Interfaces.IUserSegmentationService, Chatbot.API.Services.Phase3.UserSegmentationService>();
    builder.Services.AddScoped<Chatbot.API.Services.Phase3.Interfaces.ISearchService, Chatbot.API.Services.Phase3.SearchService>();
    builder.Services.AddScoped<Chatbot.API.Services.Phase3.Interfaces.IAnalyticsExportService, Chatbot.API.Services.Phase3.AnalyticsExportService>();
}
catch (Exception ex)
{
    // Design-time context resolution may fail during migration generation
    // This is safe to suppress as migrations can still be generated
    Console.WriteLine($"Note: Phase 3 services skipped during startup: {ex.Message}");
}

// Add HttpClientFactory for webhook delivery
builder.Services.AddHttpClient();
// Add memory cache for distributed cache interface
builder.Services.AddMemoryCache();
builder.Services.AddScoped<IDistributedCache, MemoryDistributedCache>();

// Add CORS with SignalR support
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policyBuilder =>
        {
            policyBuilder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });

    // Separate CORS policy for SignalR (allows credentials)
    options.AddPolicy("SignalRCors",
        policyBuilder =>
        {
            policyBuilder.WithOrigins("http://localhost:3000", "https://localhost:3000") // Add your client URLs
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .AllowCredentials();
        });
});

var app = builder.Build();

// Apply migrations
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ChatbotDbContext>();
    // Check if we're using InMemory database (for testing)
    var databaseName = db.Database.ProviderName;
    if (databaseName == "Microsoft.EntityFrameworkCore.InMemory")
    {
        // For InMemory database (testing), just ensure it's created
        db.Database.EnsureCreated();
    }
    else
    {
        // For relational databases (SQLite, SQL Server, etc.), run migrations
        db.Database.Migrate();
    }
}

// Configure the HTTP request pipeline
// Exception handling middleware should be first
app.UseExceptionHandling();

// Request/Response logging (after exception handling)
if (app.Environment.IsDevelopment())
{
    app.UseRequestResponseLogging();
}

// Rate limiting
app.UseRateLimiting();

// Serve static files (needed for Swagger UI)
app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();

// Add Phase 2 security middleware
app.UseMiddleware<ApiKeyValidationMiddleware>();
app.UseMiddleware<IpWhitelistEnforcementMiddleware>();

app.UseAuthorization();
app.MapControllers();

// Map SignalR Hub
app.MapHub<ChatHub>("/hubs/chat").RequireCors("SignalRCors");

app.Run();

// Make Program class accessible for integration tests
public partial class Program { }
