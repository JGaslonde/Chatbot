using Chatbot;

Console.WriteLine("╔════════════════════════════════════════╗");
Console.WriteLine("║     Welcome to C# Chatbot Console     ║");
Console.WriteLine("╚════════════════════════════════════════╝");
Console.WriteLine();

// Create chatbot instance
var chatBot = new ChatBot("Assistant");

Console.WriteLine($"Hi! I'm {chatBot.Name}, your AI assistant.");
Console.WriteLine("Type 'exit' or 'quit' to end the conversation.");
Console.WriteLine("Type 'history' to view recent messages with analysis.");
Console.WriteLine("Type 'clear' to clear conversation history.");
Console.WriteLine("Type 'analyze <message>' to analyze a message.");
Console.WriteLine();

// Main conversation loop
bool running = true;
while (running)
{
    // Display prompt
    Console.Write("You: ");
    string? userInput = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(userInput))
    {
        continue;
    }

    // Check for special commands
    string lowerInput = userInput.ToLower().Trim();
    
    if (lowerInput is "exit" or "quit")
    {
        Console.WriteLine($"\n{chatBot.Name}: {chatBot.ProcessMessage("goodbye")}");
        running = false;
        continue;
    }

    if (lowerInput == "history")
    {
        chatBot.ShowHistory();
        continue;
    }

    if (lowerInput == "clear")
    {
        chatBot.ClearHistory();
        continue;
    }

    if (lowerInput.StartsWith("analyze "))
    {
        string messageToAnalyze = userInput.Substring(8).Trim();
        if (!string.IsNullOrWhiteSpace(messageToAnalyze))
        {
            chatBot.AnalyzeMessage(messageToAnalyze);
        }
        else
        {
            Console.WriteLine("Usage: analyze <message>");
        }
        continue;
    }

    // Process message and get response
    string response = chatBot.ProcessMessage(userInput);
    Console.WriteLine($"{chatBot.Name}: {response}");
    Console.WriteLine();
}

Console.WriteLine("\nThank you for chatting! Goodbye!");

