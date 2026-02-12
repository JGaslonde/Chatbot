using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Chatbot.API.Data.Context;
using Chatbot.API.Data.Repositories;
using Chatbot.API.Services.Core;
using Chatbot.API.Services.Analysis;
using Chatbot.API.Services.Processing;
using Chatbot.API.Services.Export;
using Chatbot.API.Services.Analytics;
using Chatbot.API.Middleware;
using Chatbot.API.Hubs;
using Chatbot.API.Infrastructure.Auth;
using Chatbot.API.Infrastructure.Authorization;
using Chatbot.API.Infrastructure.Http;
using Chatbot.API.Infrastructure.Facades;
using Chatbot.API.Infrastructure.Repository;
using Chatbot.Core.Models.Entities;
using FluentValidation;
using FluentValidation.AspNetCore;
using Chatbot.API.Validators;

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

// Register repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IConversationRepository, ConversationRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
// Register generic repositories for new entities
builder.Services.AddScoped<Repository<Message>>();
builder.Services.AddScoped<Repository<User>>();
builder.Services.AddScoped<Repository<Conversation>>();
builder.Services.AddScoped<Repository<UserPreferences>>();

// Register infrastructure services (DRY and Dependency Inversion)
builder.Services.AddScoped<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<IUserContextProvider, UserContextProvider>();
builder.Services.AddScoped<IApiResponseBuilder, ApiResponseBuilder>();
builder.Services.AddScoped<IConversationAccessControl, ConversationAccessControl>();
builder.Services.AddScoped<IChatFacadeService, ChatFacadeService>();

// Register application services
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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Map SignalR Hub
app.MapHub<ChatHub>("/hubs/chat").RequireCors("SignalRCors");

app.Run();

// Make Program class accessible for integration tests
public partial class Program { }
