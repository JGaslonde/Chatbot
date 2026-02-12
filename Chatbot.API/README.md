# Chatbot.API

ASP.NET Core Web API for the C# Chatbot solution.

## Overview

RESTful API endpoints for chatbot interactions with Swagger/OpenAPI documentation.

## Project Structure

- **Controllers/** - API endpoint controllers
  - `ChatController.cs` - Chat message handling
- **Properties/launchSettings.json** - Application launch configuration
- **appsettings.json** - Configuration settings
- **Program.cs** - Application entry point and service configuration

## Features

- RESTful API endpoints
- Swagger/OpenAPI documentation
- CORS enabled for cross-origin requests
- Health check endpoint
- Ready for integration with chatbot logic

## Getting Started

### Build

```bash
dotnet build
```

### Run

```bash
dotnet run
```

The API will start on:

- HTTP: http://localhost:5089
- HTTPS: https://localhost:7089
- Swagger UI: https://localhost:7089/swagger

### API Endpoints

#### Health Check

- **GET** `/api/chat/health` - Returns API health status

#### Send Message

- **POST** `/api/chat/send` - Send a message to the chatbot
  - Request body: `{ "message": "Your message here" }`
  - Response: `{ "message": "Bot response", "timestamp": "2024-01-01T00:00:00Z" }`

## Integration

To integrate with the console chatbot:

1. Add a reference to the `Chatbot` project
2. Update `ChatController.cs` to use actual chatbot logic instead of echo responses
3. Implement persistent conversation state management
4. Add authentication and rate limiting as needed

## Dependencies

- ASP.NET Core 8.0
- Swashbuckle.AspNetCore (Swagger/OpenAPI)
