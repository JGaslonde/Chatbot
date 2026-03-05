# CLAUDE.md — Chatbot Codebase Reference

This file provides AI assistants with a comprehensive reference for the structure, conventions, and workflows of this repository.

---

## Project Overview

A multi-tier ASP.NET Core 8 chatbot application with the following key features:

- JWT-based authentication
- Real-time messaging via SignalR WebSockets
- Sentiment analysis and intent recognition
- Conversation history, analytics, and export
- SQLite persistence via Entity Framework Core
- Blazor Interactive Server web frontend
- Docker deployment support

---

## Solution Structure

The solution (`Chatbot.sln`) contains five projects:

| Project | Type | Purpose |
|---|---|---|
| `Chatbot.Core` | Class Library | Shared models and DTOs — no dependencies on other tiers |
| `Chatbot` | Console App | CLI chatbot for local interactive use |
| `Chatbot.API` | ASP.NET Core API | Primary REST API (production tier) |
| `Chatbot.Web` | Blazor Server | Interactive web frontend |
| `Chatbot.API.Tests` | xUnit Test Project | Unit and integration tests |

---

## Directory Structure

```
Chatbot/
├── Chatbot.sln
├── Dockerfile
├── docker-compose.yml
├── .github/
│   └── copilot-instructions.md
│
├── Chatbot.Core/                        # Shared models (no project dependencies)
│   └── Models/
│       ├── Entities/DataModels.cs       # Entity classes: User, Conversation, Message, UserPreferences
│       └── ApiModels.cs                 # Request/Response DTOs
│
├── Chatbot/                             # Console application
│   ├── Program.cs
│   ├── ChatBot.cs
│   ├── Conversation.cs
│   └── Services/                        # Lightweight service implementations
│
├── Chatbot.API/                         # REST API (primary project)
│   ├── Program.cs                       # DI registration, middleware pipeline, startup
│   ├── appsettings.json                 # SQLite connection string, JWT config, logging
│   ├── appsettings.Development.json
│   ├── Controllers/Chat/
│   │   ├── ChatController.cs            # All REST endpoints
│   │   └── ApiControllerBase.cs         # Shared controller base
│   ├── Services/
│   │   ├── Core/                        # ConversationService, AuthenticationService, UserPreferencesService
│   │   ├── Analysis/                    # SentimentAnalysisService, IntentRecognitionService, MessageAnalyticsService
│   │   ├── Processing/                  # MessageFilterService, ResponseTemplateService, ConversationSummarizationService
│   │   ├── Analytics/                   # ConversationAnalyticsService
│   │   └── Export/                      # ConversationExportService
│   ├── Data/
│   │   ├── Context/ChatbotDbContext.cs  # EF Core DbContext
│   │   └── Repositories/Repository.cs  # Generic repository pattern
│   ├── Hubs/ChatHub.cs                  # SignalR real-time hub
│   ├── Middleware/                      # ExceptionHandlingMiddleware, RequestResponseLoggingMiddleware, RateLimitingMiddleware
│   ├── Infrastructure/
│   │   ├── Facades/ChatFacadeService.cs # Unified service interface for controllers
│   │   ├── Http/ApiResponseBuilder.cs   # Standardized API response construction
│   │   ├── Auth/UserContextProvider.cs  # Current-user context extraction
│   │   └── Authorization/ConversationAccessControl.cs
│   ├── Validators/RequestValidators.cs  # FluentValidation rules
│   ├── Exceptions/CustomExceptions.cs   # ValidationException, NotFoundException, UnauthorizedException
│   ├── Configuration/ApiConfiguration.cs
│   └── Migrations/                      # EF Core migration files
│
├── Chatbot.Web/                         # Blazor web frontend
│   ├── Components/
│   │   ├── Pages/                       # Chat, Home, Login, Register, Analytics, History, Preferences, Profile
│   │   ├── Layout/                      # MainLayout, NavMenu
│   │   └── Shared/                      # ChatMessage, ChatInput, Alert, ConfirmDialog, FilterPanel, SearchBox
│   └── Services/                        # AuthService, ChatService, ConversationService, ChatHubService, ThemeService, etc.
│
└── Chatbot.API.Tests/
    ├── Services/                        # Unit tests: Auth, Sentiment, Intent, MessageFilter
    └── Integration/ChatApiIntegrationTests.cs
```

---

## Essential Commands

```bash
# Build entire solution
dotnet build

# Run the REST API (HTTP: 5089, HTTPS: 7089, Swagger: /swagger)
dotnet run --project Chatbot.API

# Run the Blazor web UI
dotnet run --project Chatbot.Web

# Run the console app
dotnet run --project Chatbot

# Run all tests
dotnet test

# Run only the API test project
dotnet test Chatbot.API.Tests

# Docker
docker-compose up
docker build -t chatbot-api:latest .

# EF Core migrations
dotnet ef migrations add <MigrationName> --project Chatbot.API
dotnet ef database update --project Chatbot.API
```

---

## Configuration

**`Chatbot.API/appsettings.json`** — primary config:
- `ConnectionStrings.DefaultConnection` — SQLite path (`chatbot.db`)
- `Jwt.Key` — signing key (placeholder; **must be replaced before production**)
- `Jwt.Issuer` / `Jwt.Audience` / `Jwt.ExpireMinutes` (default: 1440 = 24 hours)
- `Logging.LogLevel`

The SQLite database file `chatbot.db` is created automatically on first run. It is listed in `.gitignore` and must never be committed.

---

## Data Layer

- **ORM:** Entity Framework Core 8 with SQLite provider
- **DbContext:** `Chatbot.API/Data/Context/ChatbotDbContext.cs`
- **Migrations:** `Chatbot.API/Migrations/` — run `dotnet ef database update` after adding migrations
- **Pattern:** Generic `Repository<T>` with typed interfaces (`IUserRepository`, `IConversationRepository`, `IMessageRepository`)

### Entities (`Chatbot.Core/Models/Entities/DataModels.cs`)

| Entity | Key Fields |
|---|---|
| `User` | Id, Username, Email, PasswordHash, DisplayName, CreatedAt, IsActive |
| `Conversation` | Id, UserId (FK), Title, StartedAt, LastMessageAt, IsActive, Summary |
| `Message` | Id, ConversationId (FK), Content, Sender, SentAt, Sentiment, SentimentScore, DetectedIntent, IsFiltered |
| `UserPreferences` | Id, UserId (FK), Language, Theme, TimeZone, notification flags, ResponseStyle |

### Enums
- `MessageSender`: User, Bot, System
- `Sentiment`: VeryNegative, Negative, Neutral, Positive, VeryPositive
- `UserRole`: User, Admin, Moderator

---

## API Endpoints

All endpoints are under `/api/chat`. Protected endpoints require `Authorization: Bearer <token>`.

| Method | Path | Auth | Description |
|---|---|---|---|
| POST | `/register` | No | Register new user |
| POST | `/login` | No | Login, returns JWT |
| POST | `/conversations` | Yes | Create new conversation |
| GET | `/{id}/history` | Yes | Get conversation message history |
| POST | `/{id}/send` | Yes | Send message to conversation |
| GET | `/{id}/export/json` | Yes | Export conversation as JSON |
| GET | `/{id}/export/csv` | Yes | Export conversation as CSV |
| GET | `/analytics` | Yes | User analytics (optional date range) |
| GET | `/analytics/sentiment-trends` | Yes | Sentiment trends (last N days) |
| GET | `/analytics/intent-distribution` | Yes | Intent distribution stats |
| GET | `/preferences` | Yes | Get user preferences |
| PUT | `/preferences` | Yes | Update user preferences |
| GET | `/health` | No | Health check |

**SignalR Hub:** `/hubs/chat`
- Client methods: `JoinConversation`, `LeaveConversation`, `SendMessage`, `SendTypingIndicator`
- Server events: `ReceiveMessage`, `UserJoined`, `UserLeft`, `UserTyping`

---

## Design Patterns

### Facade Pattern
`Chatbot.API/Infrastructure/Facades/ChatFacadeService.cs` is the **preferred injection point** in controllers. It coordinates multiple services behind a single interface, reducing constructor dependencies.

### Repository Pattern
Generic `Repository<T>` implements standard CRUD. Controllers and services depend on interfaces, not concrete implementations.

### Middleware Pipeline (ordered in `Program.cs`)
1. `ExceptionHandlingMiddleware` — catches unhandled exceptions, returns structured errors
2. `RequestResponseLoggingMiddleware` — logs all requests and responses
3. `RateLimitingMiddleware` — per-client rate limiting

### Response Wrapper
All API responses use `ApiResponse<T>` with fields: `Success`, `Message`, `Data`, `Errors`. Use `ApiResponseBuilder` (`Chatbot.API/Infrastructure/Http/`) to construct responses consistently.

### Validation
`FluentValidation` rules are defined in `Chatbot.API/Validators/RequestValidators.cs` and applied automatically via middleware. Do not add ad-hoc manual validation in controllers.

---

## Coding Conventions

- **Language:** C# 12, .NET 8.0; nullable reference types enabled; implicit usings enabled
- **Async:** Use `async`/`await` throughout. Never use `.Result` or `.Wait()`.
- **Naming:** PascalCase for classes/methods/properties; camelCase for local variables and parameters
- **Interfaces:** All services have interface contracts (e.g., `IConversationService`). Depend on interfaces, not concrete classes.
- **Exceptions:** Throw types from `Chatbot.API/Exceptions/CustomExceptions.cs` (`ValidationException`, `NotFoundException`, `UnauthorizedException`). The exception middleware handles them.
- **Logging:** Inject `ILogger<T>` and use structured logging. Do not use `Console.WriteLine` in the API.
- **DTOs:** Define request/response models in `Chatbot.Core/Models/ApiModels.cs`. Use records for immutable DTOs.

---

## Testing Conventions

- **Framework:** xUnit + FluentAssertions + Moq
- **Integration tests:** Use `WebApplicationFactory<Program>` with the in-memory EF Core provider. Tests are in `Chatbot.API.Tests/Integration/`.
- **Unit tests:** Mock dependencies via Moq. Tests are in `Chatbot.API.Tests/Services/`.
- **File layout:** Mirror the source structure (e.g., `Services/AuthenticationServiceTests.cs` tests `Services/Core/AuthenticationService.cs`).
- **Database:** Never use the real SQLite file in tests. Use `UseInMemoryDatabase` to ensure isolation.

---

## Important Notes

1. **Never commit `chatbot.db`** — the SQLite database file is gitignored.
2. **Change the JWT signing key** before any production or staging deployment. The default key in `appsettings.json` is a placeholder.
3. **CORS:** When adding new frontend origins, update the CORS policy in `Program.cs` and ensure the SignalR hub origin is also allowed.
4. **Migrations:** Any change to entity classes in `DataModels.cs` requires a new EF Core migration (`dotnet ef migrations add`) and `dotnet ef database update`.
5. **Facade first:** Prefer injecting `ChatFacadeService` in new controllers rather than individual services.
6. **No CI/CD yet:** There are no GitHub Actions workflows. Tests must be run locally with `dotnet test`.
