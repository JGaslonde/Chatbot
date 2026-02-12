# Chatbot API - Quick Start Guide

## 🎯 What's New

Six powerful features have been integrated into your chatbot API:

1. **Conversation Memory** - Track and manage conversation history
2. **Sentiment Analysis** - Understand user emotions
3. **Intent Recognition** - Detect what users want
4. **Database Persistence** - Store data permanently
5. **User Authentication** - Secure user sessions
6. **Message Filtering** - Content safety and moderation

---

## 🚀 Getting Started

### Start the Server

```bash
cd Chatbot.API
dotnet run
```

**Output:**

```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7089
```

### Access Swagger UI

Open browser and go to: **https://localhost:7089/swagger**

---

## 📋 Test the Features

### 1. Register a User

```bash
curl -X POST https://localhost:7089/api/chat/register \
  -H "Content-Type: application/json" \
  -d '{
    "username": "john_doe",
    "email": "john@example.com",
    "password": "SecurePass123!"
  }'
```

**Response:**

```json
{
  "success": true,
  "message": "User registered successfully",
  "data": {
    "token": "MTo5_...base64_token"
  }
}
```

### 2. Login

```bash
curl -X POST https://localhost:7089/api/chat/login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "john_doe",
    "password": "SecurePass123!"
  }'
```

### 3. Start a Conversation

```bash
curl -X POST https://localhost:7089/api/chat/conversations \
  -H "Content-Type: application/json" \
  -d '{
    "title": "My First Chat"
  }'
```

**Response:**

```json
{
  "success": true,
  "message": "Conversation started",
  "data": {
    "id": 1,
    "title": "My First Chat",
    "startedAt": "2026-02-12T10:30:00Z",
    "messageCount": 0,
    "summary": null
  }
}
```

### 4. Send a Message (Tests All Features!)

```bash
curl -X POST https://localhost:7089/api/chat/1/send \
  -H "Content-Type: application/json" \
  -d '{
    "message": "I love your product! It works great!"
  }'
```

**Response:**

```json
{
  "success": true,
  "message": "Message processed",
  "data": {
    "message": "Echo: I love your product! It works great!",
    "timestamp": "2026-02-12T10:35:00Z",
    "intent": "feedback",
    "intentConfidence": 0.8,
    "sentiment": "VeryPositive",
    "sentimentScore": 0.95,
    "conversationId": 1
  }
}
```

### 5. Get Conversation History

```bash
curl -X GET https://localhost:7089/api/chat/1/history \
  -H "Accept: application/json"
```

**Response:**

```json
{
  "success": true,
  "message": "History retrieved",
  "data": {
    "conversationId": 1,
    "messages": [
      {
        "id": 1,
        "content": "I love your product! It works great!",
        "sender": "User",
        "sentAt": "2026-02-12T10:35:00Z",
        "sentiment": "VeryPositive",
        "intent": "feedback",
        "sentimentScore": 0.95
      },
      {
        "id": 2,
        "content": "Echo: I love your product! It works great!",
        "sender": "Bot",
        "sentAt": "2026-02-12T10:35:01Z",
        "sentiment": "VeryPositive",
        "intent": "unknown",
        "sentimentScore": 0.95
      }
    ]
  }
}
```

---

## 🧪 Feature Testing Scenarios

### Test Sentiment Analysis

```json
{
  "message": "I hate bugs! This is terrible and useless!"
}
```

→ Expected: `"sentiment": "VeryNegative"`, `"sentimentScore": -0.95`

### Test Intent Recognition

```json
{
  "message": "How can I get help with my account?"
}
```

→ Expected: `"intent": "help"`, `"intentConfidence": 0.7+`

### Test Message Filtering

```json
{
  "message": "This message has TOOOOOO many repeated characters!!!!!!!"
}
```

→ Expected: `"IsFiltered": true`, includes repetition warning

### Test Conversation Memory

1. Send message 1: "Tell me about your features"
2. Send message 2: "Do you have email support?"
3. Get history → Both messages preserved with full analysis

---

## 📊 Database Information

### SQLite Database

- **Location:** `Chatbot.API/chatbot.db`
- **Automatically Created:** On first run
- **Test User Seed:**
  - Username: `testuser`
  - Email: `test@chatbot.local`
  - Password: `password123` (pre-hashed)

### View Database

```bash
# Using sqlite3 CLI (if installed)
sqlite3 Chatbot.API/chatbot.db ".tables"
sqlite3 Chatbot.API/chatbot.db "SELECT * FROM Users;"

# Or use Visual Studio
# Tools → Connect to Database → SQLite → chatbot.db
```

---

## 🔑 Test User Login

Pre-configured test user:

```
Username: testuser
Password: password123
```

Use this to test JWT immediately without registration.

---

## 📚 Understanding the Response Structure

All API responses follow this format:

```json
{
  "success": true|false,
  "message": "Human readable message",
  "data": { /* varies by endpoint */ },
  "errors": [ /* only on failure, list of error strings */ ]
}
```

---

## 🛠️ Configuration

Edit `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=chatbot.db"
  },
  "JwtSettings": {
    "Secret": "your-super-secret-key",
    "ExpireMinutes": 1440
  }
}
```

---

## 📈 Feature Details Summary

| Feature                | Endpoint                               | Status    |
| ---------------------- | -------------------------------------- | --------- |
| **Authentication**     | POST /register, /login                 | ✅ Active |
| **Conversations**      | POST /conversations, GET /{id}/history | ✅ Active |
| **Message Sending**    | POST /{id}/send                        | ✅ Active |
| **Sentiment Analysis** | Auto-included in responses             | ✅ Active |
| **Intent Recognition** | Auto-included in responses             | ✅ Active |
| **Message Filtering**  | Auto-included in responses             | ✅ Active |
| **Health Check**       | GET /health                            | ✅ Active |

---

## 🔍 Monitoring & Debugging

### Check Build Status

```bash
dotnet build
```

### Enable Debug Logging

Modify `appsettings.Development.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.EntityFrameworkCore": "Debug"
    }
  }
}
```

### View Active Connections

```bash
# Windows
tasklist | findstr dotnet

# Mac/Linux
ps aux | grep dotnet
```

---

## ⚡ Performance Tips

1. **Database:** Queries are indexed on frequent operations
2. **Sentiment:** Dictionary-based (fast), can upgrade to ML models
3. **Intent:** Pattern-based (fast), can upgrade to NLP
4. **Caching:** Add Redis for conversation history
5. **Async:** All operations are async/await

---

## 🔐 Security Notes

⚠️ **Important for Production:**

- Change JWT secret in appsettings.json
- Use HTTPS only (enabled by default)
- Migrate from Base64 to JWT tokens (code in place)
- Implement rate limiting
- Add request validation
- Enable HTTPS certificate verification

---

## 📞 Need Help?

1. **API Errors:** Check Swagger UI in browser
2. **Database Issues:** Verify `chatbot.db` exists in project root
3. **Authentication:** Use test user if registration fails
4. **Build Issues:** Run `dotnet clean && dotnet build`
5. **Port Conflicts:** Change port in `Properties/launchSettings.json`

---

## 🎓 Architecture Overview

```
Request → Controller → Service → Repository → Database
                      ↓
              Sentiment Analysis
              Intent Recognition
              Message Filtering
                      ↓
              Response (with enriched data)
```

Each component is independently testable and maintainable.

---

**Happy Chatting! 🚀**

For detailed implementation info, see [IMPLEMENTATION_SUMMARY.md](../IMPLEMENTATION_SUMMARY.md)
