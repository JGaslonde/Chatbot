# Quick Start Guide - Expanded Console Application

## 🚀 Get Started in 5 Minutes

### Step 1: Open Two Terminal Windows

```powershell
# Both from: D:\OneDrive\Documents\source\repos\Chatbot
```

### Step 2: Start the API (Terminal 1)

```powershell
cd Chatbot.API
dotnet run
```

**Wait for the message:**

```
Now listening on: http://localhost:5089
press CTRL+C to shut down
```

### Step 3: Start the Console (Terminal 2)

```powershell
cd Chatbot
dotnet run
```

**You should see:**

```
╔════════════════════════════════════════════════════════════╗
║         Welcome to Advanced C# Chatbot Console             ║
║                                                            ║
║  Features: Remote API Integration • Authentication        ║
║            Analytics • Search • Export • Enterprise APIs  ║
╚════════════════════════════════════════════════════════════╝

Initializing API client...
✓ API server connected successfully!

╔════════════════════════════════════════════════════════════╗
║  Smart Chatbot Console - Login/Register                   ║
╚════════════════════════════════════════════════════════════╝

1. Login
2. Register
3. Try as Guest (Local Mode)
4. Exit

Choose an option: _
```

### Step 4: Create Your Account (First Time)

```
Choose an option: 2

Enter username: john_smith
Enter email: john@example.com
Enter password: ••••••••

✓ Registration successful!
```

### Step 5: Explore Features

#### Feature 1: Start a Conversation

```
Choose an option: 1

Conversation started (ID: f47ac10b-58cc-4372-a567-0e02b2c3d479)
Type 'back' to return to menu, 'export' to save conversation

You: Hello! What can you do?
✓ Assistant: I'm an AI assistant that can help with various tasks...

You: Can you analyze sentiment?
✓ Assistant: Yes, I can analyze the emotional tone of text...

You: back
```

#### Feature 2: View Your Conversations

```
Choose an option: 2

YOUR CONVERSATIONS

1. Conversation from 2/14/2026 (15 messages)
   Created: 2/14/2026 10:30 AM
   Last message: 2/14/2026 11:05 AM

2. Conversation from 2/13/2026 (8 messages)
   Created: 2/13/2026 3:45 PM
   Last message: 2/13/2026 3:51 PM

Select a conversation (number) or press 0 to go back: 1

CONVERSATION HISTORY

User: Hello! What can you do?
  [2/14/2026 10:32 AM]
```
