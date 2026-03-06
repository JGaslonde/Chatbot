# Advanced C# Chatbot Console Application

## Overview

The console application has been significantly expanded to fully utilize all features available in the API. It now offers both local chatbot capabilities and remote API integration with enterprise-grade features.

## Features

### 🔐 Authentication & User Management

- **User Registration** - Create new user accounts with email verification
- **User Login** - Secure JWT-based authentication
- **Guest Mode** - Use local chatbot without account
- **Session Management** - Maintain authenticated sessions

### 💬 Conversation Management

- **Create Conversations** - Start new chat sessions with the API
- **View Conversations** - Browse conversation history
- **Search Conversations** - Full-text search across all conversations
- **Conversation History** - View complete message history with timestamps
- **Multiple Conversations** - Manage and switch between multiple active conversations

### 📊 Analytics & Insights

- **Conversation Analytics** - View detailed analytics for each conversation
- **Message Sentiment Analysis** - Track sentiment trends across messages
- **Intent Recognition** - Analyze user intents from conversation history
- **Performance Metrics** - View engagement and response quality metrics

### 🔍 Advanced Search

- **Full-Text Search** - Search messages across all conversations
- **Conversation Search** - Find specific conversations by content
- **Quick Filtering** - Filter results by date, sentiment, intent

### ⚙️ User Preferences

- **Preference Management** - Customize bot behavior and appearance
- **Language Settings** - Select preferred language
- **Notification Settings** - Control notification preferences
- **Theme Preferences** - Choose interface theme

### 📤 Data Export

- **CSV Export** - Export conversations in CSV format
- **JSON Export** - Export conversations in JSON format
- **Bulk Export** - Export multiple conversations at once
- **Scheduled Exports** - Set up recurring exports

### 🔔 Notifications

- **Unread Notifications** - View pending notifications
- **Notification Management** - Mark notifications as read
- **Multi-Channel Alerts** - Receive notifications via multiple channels
- **Alert Preferences** - Configure notification rules

### 🚀 Enterprise Features

#### Webhooks

- **View Webhooks** - List all registered webhooks
- **Create Webhooks** - Set up event-driven integrations
- **Manage Deliveries** - Track webhook event delivery history
- **Retry Logic** - Automatic retry with exponential backoff
- **Signature Validation** - Cryptographic signature verification

#### API Keys

- **Generate API Keys** - Create new API keys for integrations
- **View API Keys** - List all generated keys
- **Rotate Keys** - Securely rotate API keys
- **Rate Limiting** - Configure rate limits per key

#### Two-Factor Authentication (2FA)

- **Enable 2FA** - Set up two-factor authentication
- **Setup TOTP** - Configure Time-based One-Time Passwords
- **Recovery Codes** - Generate and manage backup codes
- **Disable 2FA** - Turn off two-factor authentication

#### IP Whitelist Management

- **View Whitelist** - List approved IP addresses
- **Add IPs** - Add new IP addresses to whitelist
- **Remove IPs** - Remove IPs from whitelist
- **Access Control** - Restrict access by IP address

#### Reporting

- **Generate Reports** - Create custom reports
- **Scheduled Reports** - Set up automatic report generation
- **Email Delivery** - Receive reports via email
- **Multiple Formats** - Export reports in various formats

#### Import/Export Jobs

- **Import Data** - Bulk import conversations and messages
- **Export Jobs** - Track export operations
- **Job Status** - Monitor import/export progress
- **Error Handling** - View and manage job errors

#### Caching

- **Manage Cache** - View and clear cached data
- **Cache Statistics** - Monitor cache performance
- **Cache Configuration** - Customize caching behavior

#### Batch Operations

- **Batch Archive** - Archive multiple conversations at once
- **Batch Delete** - Delete multiple conversations
- **Bulk Updates** - Update multiple conversations efficiently
- **Progress Tracking** - Monitor batch operation progress

#### Audit Logging

- **View Audit Logs** - Track all system activities
- **Compliance Reporting** - Generate compliance reports
- **Activity Filtering** - Filter logs by user, action, date
- **Export Logs** - Export audit logs for analysis

## Getting Started

### Prerequisites

- .NET 8.0+ installed
- Chatbot API running (see API documentation)
- Visual Studio, VS Code, or command line

### Running the Console Application

**Option 1: With API (Full Features)**

```bash
# Terminal 1: Start the API
cd Chatbot.API
dotnet run

# Terminal 2: Start the Console (after API is ready)
cd Chatbot
dotnet run
```

**Option 2: Local Mode Only (Limited Features)**

```bash
cd Chatbot
dotnet run
```

### First Time User

1. **Start the application** - `dotnet run`
2. **Choose authentication method**:
   - `1` - Login with existing account
   - `2` - Register new account
   - `3` - Use Guest Mode (local chatbot)
   - `4` - Exit
3. **Choose from main menu** - Select desired feature

## Main Menu

```
═══════════════════════════════════════════════════════════════════╗
║           Welcome to Advanced C# Chatbot Console                 ║
║                                                                  ║
║  Features: Remote API Integration • Authentication              ║
║            Analytics • Search • Export • Enterprise APIs         ║
╚═══════════════════════════════════════════════════════════════════╝

1. Start New Conversation      - Begin a new remote chat session
2. View Conversations          - Browse your conversation history
3. Search Conversations        - Find conversations by content
4. View Analytics              - See detailed analytics and insights
5. Manage Preferences          - Customize your settings
6. View Notifications          - Check unread notifications
7. Export Conversation         - Save conversations to CSV/JSON
8. Enterprise Features         - Access advanced enterprise tools
9. Test API Health             - Verify API connection status
0. Logout                       - Sign out and return to login
```

## Enterprise Features Menu

```
═══════════════════════════════════════════════════════════════════╗
║                    ENTERPRISE FEATURES                           ║
╚═══════════════════════════════════════════════════════════════════╝

1. View Webhooks               - Manage event-driven integrations
2. Two-Factor Authentication   - Enable/manage 2FA
3. Manage API Keys             - Create and rotate API keys
4. IP Whitelist                - Control access by IP
5. Generate Reports            - Create custom reports
6. Back to Main Menu            - Return to main menu
```

## Usage Examples

### Starting a New Conversation

```
1. Select "Start New Conversation" from main menu
2. Chat naturally with the bot
3. Type 'export' to save the conversation
4. Type 'back' to return to menu
```

### Searching Your Conversations

```
1. Select "Search Conversations"
2. Enter search query (e.g., "weather", "python", "help")
3. Review search results with message counts
4. Select conversation to view full history
```

### Exporting Data

```
1. Select "Export Conversation"
2. Choose format (CSV or JSON)
3. Export is generated and displayed
4. Save results to file manually via copy-paste
```

### Viewing Analytics

```
1. Select "View Analytics" (must have conversation selected)
2. View sentiment trends, intent distribution, metrics
3. Analyze conversation patterns and engagement
```

### Managing Preferences

```
1. Select "Manage Preferences"
2. View current preference settings
3. Modify settings as needed (theme, language, notifications)
```

## API Connection Details

The console automatically attempts to connect to the API at:

- **URL**: `http://localhost:5089`
- **Port**: `5089`
- **Protocol**: HTTP/HTTPS

### Setting Custom API URL

To connect to a different API server, modify `Program.cs`:

```csharp
// Change from:
var apiClient = new ApiClient("http://localhost:5089");

// To:
var apiClient = new ApiClient("https://your-api-server.com:8443");
```

## Error Handling

The application includes comprehensive error handling:

- **Network errors** - Graceful fallback to local mode
- **Authentication failures** - Clear error messages with retry options
- **API errors** - Appropriate error messages and guidance
- **Validation errors** - Input validation with helpful prompts

## Local Chatbot Features

When running in Guest Mode or when API is unavailable:

- **Sentiment Analysis** - Detect message sentiment (positive/negative/neutral)
- **Intent Recognition** - Identify user intent from messages
- **Response Generation** - Generate contextual responses
- **Message Filtering** - Filter inappropriate content
- **Conversation History** - Maintain local conversation history
- **Message Analysis** - Analyze individual messages for insights

## Security Considerations

1. **Passwords** - Entered passwords are masked and never logged
2. **Tokens** - JWT tokens are stored in memory during session
3. **HTTPS** - Use HTTPS when connecting to remote servers
4. **API Keys** - Keep enterprise API keys confidential
5. **2FA** - Enable two-factor authentication for enhanced security

## Keyboard Shortcuts

- **Ctrl+C** - Exit application (graceful shutdown)
- **Backspace** - Correct password input (while typing)
- **Enter** - Confirm input

## Troubleshooting

### API Connection Failed

**Problem**: "API is not responding at http://localhost:5089"

**Solution**:

1. Verify API is running: `cd Chatbot.API && dotnet run`
2. Check port 5089 is available
3. Verify Windows Firewall allows localhost connections
4. Try again after API startup completes

### Authentication Errors

**Problem**: "Login failed" or "Registration failed"

**Solution**:

1. Check username/password are correct
2. Verify API is running
3. Try registering a new account
4. Check API logs for detailed error information

### Export Not Working

**Problem**: Export returns empty or errors

**Solution**:

1. Ensure a conversation is selected
2. Verify API is running
3. Check conversation has messages
4. Try CSV format if JSON fails

### Slow Performance

**Problem**: Console responds slowly to inputs

**Solution**:

1. Check API server load
2. Reduce number of loaded conversations
3. Clear browser cache if using Swagger UI
4. Check network connection quality

## Development

### Project Structure

```
Chatbot/
├── Program.cs                 # Application entry point
├── ChatBot.cs                 # Local chatbot logic
├── Conversation.cs            # Conversation management
├── Chatbot.csproj            # Project file
└── Services/
    ├── ApiClient.cs          # API communication
    ├── ConsoleMenu.cs        # Menu system
    ├── SentimentAnalyzer.cs  # Sentiment analysis
    ├── IntentRecognizer.cs   # Intent recognition
    └── MessageFilter.cs      # Content filtering
```

### Architecture

The console uses a modular architecture:

```
Program.cs (Entry Point)
    ↓
ConsoleMenu (Menu System)
    ↓
ApiClient (API Communication)  ← → Chatbot API
    ↓
Local Services (When Offline)
```

## Building from Source

```bash
# Build console project only
cd Chatbot
dotnet build

# Build entire solution
cd ..
dotnet build

# Run in debug mode
dotnet run -- --debug
```

## Future Enhancements

- [ ] Persistent session storage
- [ ] Configuration file for API settings
- [ ] Command-line arguments for automation
- [ ] Batch processing capabilities
- [ ] Advanced data visualization
- [ ] Plugin system for extensions
- [ ] Web endpoint for remote access
- [ ] GraphQL support
- [ ] Real-time notifications
- [ ] Voice input/output support

## Support & Documentation

- **API Documentation**: http://localhost:5089/swagger
- **Project README**: [../README.md](../README.md)
- **API Readme**: [../Chatbot.API/README.md](../Chatbot.API/README.md)

## License

This project is part of the Chatbot solution and follows the same licensing terms.
