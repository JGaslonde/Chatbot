# C# Chatbot Solution

A comprehensive C# solution with a console-based chatbot application and a RESTful API for chatbot interactions.

## Project Overview

### 1. Chatbot (Console Application)

Interactive command-line chatbot with advanced AI features including sentiment analysis, intent recognition, and message filtering.

**Location:** `Chatbot/`

**Features:**
- Conversation history tracking with sentiment and intent analysis
- Real-time sentiment analysis (VeryPositive, Positive, Neutral, Negative, VeryNegative)
- Intent recognition (greeting, farewell, help, question, command, feedback, thanks)
- Message filtering and content moderation
- Interactive commands (history, clear, analyze)

**Files:**
- **Program.cs** - Interactive console interface
- **ChatBot.cs** - Core chatbot logic with AI integration
- **Conversation.cs** - Conversation history management
- **Services/SentimentAnalyzer.cs** - Sentiment analysis service
- **Services/IntentRecognizer.cs** - Intent recognition service
- **Services/MessageFilter.cs** - Message filtering service
- **Chatbot.csproj** - Project file

**Run:** `dotnet run --project Chatbot`

**Commands:**
- Type any message to chat
- `history` - View conversation history with analysis
- `clear` - Clear conversation history
- `analyze <message>` - Analyze a specific message
- `exit` or `quit` - End the conversation

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

**Example Session:**

```
╔════════════════════════════════════════╗
║     Welcome to C# Chatbot Console     ║
╚════════════════════════════════════════╝

Hi! I'm Assistant, your AI assistant.
Type 'exit' or 'quit' to end the conversation.
Type 'history' to view recent messages with analysis.
Type 'clear' to clear conversation history.
Type 'analyze <message>' to analyze a message.

You: Hello!
Assistant: Hi there! What's on your mind?

You: I love this chatbot!
Assistant: Glad I could assist you!

You: analyze I hate bugs
=== Message Analysis ===
Message: I hate bugs
Sentiment: VeryNegative (Score: -0.90)
Intent: unknown (Confidence: 0.00)
Filtered: False
=======================

You: history
=== Recent Conversation History ===
[16:20:00] User: Hello! [Sentiment: Neutral, Intent: greeting]
[16:20:00] Assistant: Hi there! What's on your mind?
[16:20:15] User: I love this chatbot! [Sentiment: VeryPositive, Intent: greeting]
[16:20:15] Assistant: Glad I could assist you!
===================================

You: exit
Assistant: Goodbye! Have a great day!
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
├── Chatbot.sln                     # Solution file
├── .gitignore                      # Git ignore rules
├── README.md                       # This file
├── FEATURE_PLAN.md                 # Feature planning document
├── IMPLEMENTATION_SUMMARY.md       # Detailed implementation guide
├── QUICK_START.md                  # Quick start guide
├── .github/
│   └── copilot-instructions.md    # Copilot instructions
├── Chatbot/                        # Console app project ✨ NEW
│   ├── Program.cs                  # Interactive console interface
│   ├── ChatBot.cs                  # Core chatbot with AI features
│   ├── Conversation.cs             # Conversation history
│   ├── Services/
│   │   ├── SentimentAnalyzer.cs   # Sentiment analysis
│   │   ├── IntentRecognizer.cs    # Intent recognition
│   │   └── MessageFilter.cs       # Message filtering
│   └── Chatbot.csproj
└── Chatbot.API/                    # API project
    ├── Program.cs
    ├── appsettings.json
    ├── appsettings.Development.json
    ├── chatbot.db                  # SQLite database
    ├── Controllers/
    │   └── ChatController.cs
    ├── Services/
    │   ├── ConversationService.cs
    │   ├── SentimentAnalysisService.cs
    │   ├── IntentRecognitionService.cs
    │   ├── MessageFilterService.cs
    │   └── AuthenticationService.cs
    ├── Data/
    │   ├── ChatbotDbContext.cs
    │   └── Repository.cs
    ├── Models/
    │   ├── Entities/
    │   ├── Requests/
    │   └── Responses/
    ├── Properties/
    │   └── launchSettings.json
    └── Chatbot.API.csproj
```

## Technology Stack

- **.NET:** 8.0
- **Framework:** ASP.NET Core (for API)
- **Language:** C# with nullable reference types
- **Database:** SQLite with Entity Framework Core
- **Authentication:** BCrypt password hashing
- **Validation:** FluentValidation
- **API Documentation:** Swagger/OpenAPI
- **Web:** ASP.NET Core with CORS support
- **AI Features:** Sentiment analysis, intent recognition, message filtering, response templates

## Features Implemented

### Console Application
- ✅ Interactive conversation interface
- ✅ Sentiment analysis (VeryPositive to VeryNegative)
- ✅ Intent recognition (greeting, farewell, help, question, etc.)
- ✅ Message filtering and content moderation
- ✅ Conversation history with analysis
- ✅ Real-time message analysis

### API Application

#### Core Features (2024-2025)
- ✅ RESTful endpoints with Swagger documentation
- ✅ User authentication and registration
- ✅ Conversation management with persistence
- ✅ Database persistence (SQLite with EF Core migrations)
- ✅ Sentiment analysis service
- ✅ Intent recognition service
- ✅ Message filtering service
- ✅ Health check endpoint

#### Advanced Features (February 2026 - Phase 1)
- ✅ **Response Templates & Context-Aware Responses** - Intelligent, adaptive conversation
- ✅ **Conversation Summarization** - Auto-generated titles and summaries
- ✅ **Rate Limiting** - IP-based throttling (100 req/min)
- ✅ **Enhanced Error Handling** - Custom exceptions with global middleware
- ✅ **Request/Response Logging** - Full audit trail with data masking
- ✅ **FluentValidation** - Robust input validation

#### New Features (February 2026 - Phase 2) 🆕
- ✅ **Comprehensive Testing** - xUnit with unit and integration tests (33+ passing)
- ✅ **JWT Authentication** - Industry-standard token-based security
- ✅ **Real-Time WebSockets** - SignalR for instant messaging and typing indicators
- ✅ **Docker Containerization** - Production-ready deployment with Docker Compose

📖 **See [NEW_FEATURES.md](NEW_FEATURES.md) for Phase 1 features (2026-02-11)**  
📖 **See [NEW_FEATURES_2026.md](NEW_FEATURES_2026.md) for Phase 2 features (2026-02-12)**

## Customization Points

### Console Chatbot

- Extend `ChatBot.cs` with new conversational logic
- Add more sentiment words to `SentimentAnalyzer.cs`
- Add more intent patterns to `IntentRecognizer.cs`
- Customize filtering rules in `MessageFilter.cs`
- Add persistence for conversation history
- Integrate with the Chatbot.API for cloud storage

### Chatbot.API

- Extend response templates in `ResponseTemplateService.cs`
- Customize conversation summarization algorithms
- Adjust rate limiting policies per endpoint
- Add more custom exception types as needed
- Enhance validation rules for specific use cases
- Configure production logging and monitoring

## Next Steps

### Production Readiness
1. ✅ Enhanced error handling and validation
2. ✅ Rate limiting and throttling
3. ✅ Request/response logging
4. ✅ **Unit tests and integration tests (Phase 2 - NEW)**
5. ✅ **JWT authentication (Phase 2 - NEW)**
6. ✅ **Docker containerization (Phase 2 - NEW)**
7. 🔲 Switch to production database (SQL Server/PostgreSQL)
8. 🔲 Redis for distributed caching and rate limiting
9. 🔲 CI/CD pipeline setup
10. 🔲 Cloud deployment (Azure/AWS)

### Feature Enhancements
1. ✅ **Real-time WebSocket support (Phase 2 - NEW)**
2. 🔲 Machine learning for better sentiment/intent recognition
3. 🔲 Multi-language support
4. 🔲 Conversation analytics dashboard
5. 🔲 A/B testing for response templates
6. 🔲 Voice input/output support
7. 🔲 File attachment support
8. 🔲 Advanced conversation branching

## License

[Add your license here if applicable]
