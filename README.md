# C# Chatbot Solution

A comprehensive C# solution with a console-based chatbot application and a RESTful API for chatbot interactions.

## Project Overview

### 1. Chatbot (Console Application)

Interactive command-line chatbot with conversation history tracking.

**Location:** `Chatbot/`

- **Program.cs** - Application entry point
- **ChatBot.cs** - Core chatbot logic
- **Conversation.cs** - Conversation history management
- **Chatbot.csproj** - Project file

**Run:** `dotnet run --project Chatbot`

### 2. Chatbot.API (ASP.NET Core Web API)

RESTful API endpoints for chatbot interactions with Swagger documentation.

**Location:** `Chatbot.API/`

- **Controllers/ChatController.cs** - API endpoints
- **Program.cs** - Application configuration
- **Properties/launchSettings.json** - Launch configuration
- **appsettings.json** - Configuration settings

**Run:** `dotnet run --project Chatbot.API`
**Swagger UI:** https://localhost:7089/swagger

## Getting Started

### Build the Solution

```bash
dotnet build
```

### Run the Console Chatbot

```bash
dotnet run --project Chatbot
```

### Run the API

```bash
dotnet run --project Chatbot.API
```

## API Endpoints

### Health Check

```
GET /api/chat/health
```

Returns API status.

### Send Message

```
POST /api/chat/send
Content-Type: application/json

{
  "message": "Your message here"
}
```

Returns:

```json
{
  "message": "Bot response",
  "timestamp": "2024-01-01T00:00:00Z"
}
```

## Development Workflow

1. **Build:** `dotnet build`
2. **Run Tests:** `dotnet test` (when tests are added)
3. **Console Run:** `dotnet run --project Chatbot`
4. **API Run:** `dotnet run --project Chatbot.API`

## Project Structure

```
Chatbot/
├── Chatbot.sln                  # Solution file
├── .gitignore                   # Git ignore rules
├── .github/
│   └── copilot-instructions.md # Copilot instructions
├── Chatbot/                     # Console app project
│   ├── Program.cs
│   ├── ChatBot.cs
│   ├── Conversation.cs
│   └── Chatbot.csproj
└── Chatbot.API/                 # API project
    ├── Program.cs
    ├── appsettings.json
    ├── appsettings.Development.json
    ├── Controllers/
    │   └── ChatController.cs
    ├── Properties/
    │   └── launchSettings.json
    ├── Chatbot.API.csproj
    └── README.md
```

## Technology Stack

- **.NET:** 8.0
- **Framework:** ASP.NET Core (for API)
- **Language:** C# with nullable reference types
- **API Documentation:** Swagger/OpenAPI
- **Web:** ASP.NET Core with CORS support

## Customization Points

### Console Chatbot

- Extend `ChatBot.cs` with new conversational logic
- Modify response patterns and AI capabilities
- Add persistence for conversation history
- Integrate with external APIs or databases

### Chatbot.API

- Update `ChatController.cs` to use actual chatbot logic
- Integrate with the Chatbot console project
- Add authentication and authorization
- Implement rate limiting and request validation
- Add database support for conversation persistence

## Next Steps

1. Implement actual chatbot logic in `ChatBot.cs`
2. Add unit tests for both projects
3. Integrate the console chatbot with the API
4. Add database persistence
5. Deploy to cloud platform (Azure, AWS, etc.)

## License

[Add your license here if applicable]
