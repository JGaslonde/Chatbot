using Microsoft.EntityFrameworkCore;
using Chatbot.API.Data;
using Chatbot.API.Services;
using Chatbot.API.Middleware;
using FluentValidation;
using FluentValidation.AspNetCore;
using Chatbot.API.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add database context
builder.Services.AddDbContext<ChatbotDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ??
    "Data Source=chatbot.db"));

// Add services to the container
builder.Services.AddControllers();

// Add FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<ChatMessageRequestValidator>();

builder.Services.AddSwaggerGen();

// Register repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IConversationRepository, ConversationRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();

// Register application services
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IConversationService, ConversationService>();
builder.Services.AddScoped<ISentimentAnalysisService, SimpleSentimentAnalysisService>();
builder.Services.AddScoped<IIntentRecognitionService, SimpleIntentRecognitionService>();
builder.Services.AddScoped<IMessageFilterService, MessageFilterService>();
builder.Services.AddScoped<IResponseTemplateService, ResponseTemplateService>();
builder.Services.AddScoped<IConversationSummarizationService, ConversationSummarizationService>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policyBuilder =>
        {
            policyBuilder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

var app = builder.Build();

// Apply migrations
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ChatbotDbContext>();
    db.Database.Migrate();
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
app.UseAuthorization();
app.MapControllers();

app.Run();
