using Chatbot.Web.Components;
using Chatbot.Web.Services;
using Chatbot.Web.Services.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add authentication
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<ApiAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider =>
    provider.GetRequiredService<ApiAuthenticationStateProvider>());

// Configure HttpClient for API
var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5000";
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseUrl) });

// Add services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
builder.Services.AddScoped<IPreferencesService, PreferencesService>();
builder.Services.AddScoped<IConversationService, ConversationService>();
builder.Services.AddScoped<IChatHubService>(sp => new ChatHubService(apiBaseUrl));
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddScoped<IThemeService, ThemeService>();

// Add Phase 2 services
builder.Services.AddScoped<IWebhookManagementService, WebhookManagementService>();
builder.Services.AddScoped<IApiKeyManagementService, ApiKeyManagementService>();
builder.Services.AddScoped<ITwoFactorManagementService, TwoFactorManagementService>();
builder.Services.AddScoped<IIpWhitelistManagementService, IpWhitelistManagementService>();
builder.Services.AddScoped<IReportingManagementService, ReportingManagementService>();
builder.Services.AddScoped<IImportManagementService, ImportManagementService>();
builder.Services.AddScoped<IUserPreferencesManagementService, UserPreferencesManagementService>();

builder.Services.AddLogging();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
