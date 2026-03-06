namespace Chatbot.Services;

/// <summary>
/// Console menu system for the chatbot application
/// </summary>
public class ConsoleMenu
{
    private readonly ApiClient _apiClient;
    private string? _currentUserId;
    private string? _currentConversationId;

    public ConsoleMenu(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public bool IsAuthenticated => !string.IsNullOrWhiteSpace(_apiClient.GetType().GetField("_authToken",
        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(_apiClient)?.ToString());

    public void Run()
    {
        bool running = true;
        while (running)
        {
            if (!IsAuthenticated)
            {
                ShowAuthenticationMenu();
            }
            else
            {
                ShowMainMenu();
            }
        }
    }

    private void ShowAuthenticationMenu()
    {
        Console.Clear();
        Console.WriteLine("╔════════════════════════════════════════╗");
        Console.WriteLine("║  Smart Chatbot Console - Login/Register║");
        Console.WriteLine("╚════════════════════════════════════════╝");
        Console.WriteLine();
        Console.WriteLine("1. Login");
        Console.WriteLine("2. Register");
        Console.WriteLine("3. Try as Guest (Local Mode)");
        Console.WriteLine("4. Exit");
        Console.WriteLine();
        Console.Write("Choose an option: ");

        string? choice = Console.ReadLine();
        switch (choice)
        {
            case "1":
                HandleLogin();
                break;
            case "2":
                HandleRegister();
                break;
            case "3":
                Console.WriteLine("\nGuest mode activated. Using local chatbot.\n");
                RunLocalChatbot();
                break;
            case "4":
                Environment.Exit(0);
                break;
            default:
                Console.WriteLine("Invalid option. Press any key to continue...");
                Console.ReadKey();
                break;
        }
    }

    private void HandleLogin()
    {
        Console.Clear();
        Console.WriteLine("╔════════════════════════════════════════╗");
        Console.WriteLine("║              USER LOGIN                ║");
        Console.WriteLine("╚════════════════════════════════════════╝");
        Console.WriteLine();

        Console.Write("Username: ");
        string? username = Console.ReadLine();

        Console.Write("Password: ");
        string? password = ReadPassword();
        Console.WriteLine();

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            Console.WriteLine("Username and password are required.");
            Console.ReadKey();
            return;
        }

        var task = Task.Run(async () => await _apiClient.LoginAsync(username, password));
        var result = task.Result;

        if (result.success)
        {
            Console.WriteLine($"✓ {result.message}");
            _currentUserId = username;
            System.Threading.Thread.Sleep(1500);
        }
        else
        {
            Console.WriteLine($"✗ Login failed: {result.message}");
            Console.ReadKey();
        }
    }

    private void HandleRegister()
    {
        Console.Clear();
        Console.WriteLine("╔════════════════════════════════════════╗");
        Console.WriteLine("║            USER REGISTRATION           ║");
        Console.WriteLine("╚════════════════════════════════════════╝");
        Console.WriteLine();

        Console.Write("Username: ");
        string? username = Console.ReadLine();

        Console.Write("Email: ");
        string? email = Console.ReadLine();

        Console.Write("Password: ");
        string? password = ReadPassword();
        Console.WriteLine();

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            Console.WriteLine("All fields are required.");
            Console.ReadKey();
            return;
        }

        var task = Task.Run(async () => await _apiClient.RegisterAsync(username, email, password));
        var result = task.Result;

        if (result.success)
        {
            Console.WriteLine($"✓ {result.message}");
            _currentUserId = username;
            System.Threading.Thread.Sleep(1500);
        }
        else
        {
            Console.WriteLine($"✗ Registration failed: {result.message}");
            Console.ReadKey();
        }
    }

    private void ShowMainMenu()
    {
        Console.Clear();
        Console.WriteLine("╔════════════════════════════════════════╗");
        Console.WriteLine($"║  Welcome {_currentUserId,-28}║");
        Console.WriteLine("║         Smart Chatbot Menu              ║");
        Console.WriteLine("╚════════════════════════════════════════╝");
        Console.WriteLine();
        Console.WriteLine("1. Start New Conversation");
        Console.WriteLine("2. View Conversations");
        Console.WriteLine("3. Search Conversations");
        Console.WriteLine("4. View Analytics");
        Console.WriteLine("5. Manage Preferences");
        Console.WriteLine("6. View Notifications");
        Console.WriteLine("7. Export Conversation");
        Console.WriteLine("8. Enterprise Features");
        Console.WriteLine("9. Test API Health");
        Console.WriteLine("0. Logout");
        Console.WriteLine();
        Console.Write("Choose an option: ");

        string? choice = Console.ReadLine();
        switch (choice)
        {
            case "1":
                StartNewConversation();
                break;
            case "2":
                ViewConversations();
                break;
            case "3":
                SearchConversations();
                break;
            case "4":
                ViewAnalytics();
                break;
            case "5":
                ManagePreferences();
                break;
            case "6":
                ViewNotifications();
                break;
            case "7":
                ExportConversation();
                break;
            case "8":
                EnterpriseMenu();
                break;
            case "9":
                TestApiHealth();
                break;
            case "0":
                _currentUserId = null;
                _currentConversationId = null;
                break;
            default:
                Console.WriteLine("Invalid option.");
                Console.ReadKey();
                break;
        }
    }

    private void StartNewConversation()
    {
        Console.Clear();
        Console.WriteLine("╔════════════════════════════════════════╗");
        Console.WriteLine("║        START NEW CONVERSATION          ║");
        Console.WriteLine("╚════════════════════════════════════════╝");
        Console.WriteLine();

        // For now, use a temporary conversation ID
        _currentConversationId = Guid.NewGuid().ToString();

        Console.WriteLine($"Conversation started (ID: {_currentConversationId})");
        Console.WriteLine("Type 'back' to return to menu, 'export' to save conversation");
        Console.WriteLine();

        bool conversing = true;
        while (conversing)
        {
            Console.Write("You: ");
            string? input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
                continue;

            if (input.Equals("back", StringComparison.OrdinalIgnoreCase))
            {
                conversing = false;
                break;
            }

            if (input.Equals("export", StringComparison.OrdinalIgnoreCase))
            {
                ExportCurrentConversation();
                continue;
            }

            var sendTask = Task.Run(async () =>
                await _apiClient.SendMessageAsync(_currentConversationId, input));
            var result = sendTask.Result;

            if (result.success)
            {
                Console.WriteLine($"✓ Assistant: {result.response}");
            }
            else
            {
                Console.WriteLine($"✗ Error: {result.response}");
            }
            Console.WriteLine();
        }
    }

    private void ViewConversations()
    {
        Console.Clear();
        Console.WriteLine("╔════════════════════════════════════════╗");
        Console.WriteLine("║         YOUR CONVERSATIONS             ║");
        Console.WriteLine("╚════════════════════════════════════════╝");
        Console.WriteLine();

        var task = Task.Run(async () => await _apiClient.GetConversationsAsync());
        var result = task.Result;

        if (!result.success)
        {
            Console.WriteLine("Failed to retrieve conversations.");
            Console.ReadKey();
            return;
        }

        if (result.conversations.Count == 0)
        {
            Console.WriteLine("No conversations found.");
            Console.ReadKey();
            return;
        }

        for (int i = 0; i < result.conversations.Count; i++)
        {
            var conv = result.conversations[i];
            Console.WriteLine($"{i + 1}. {conv.Title} ({conv.MessageCount} messages)");
            Console.WriteLine($"   Created: {conv.CreatedAt:G}");
            if (conv.LastMessageAt.HasValue)
                Console.WriteLine($"   Last message: {conv.LastMessageAt:g}");
            Console.WriteLine();
        }

        Console.Write("Select a conversation (number) or press 0 to go back: ");
        if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= result.conversations.Count)
        {
            _currentConversationId = result.conversations[choice - 1].Id;
            ViewConversationDetails();
        }
    }

    private void ViewConversationDetails()
    {
        if (string.IsNullOrWhiteSpace(_currentConversationId))
            return;

        Console.Clear();
        Console.WriteLine("╔════════════════════════════════════════╗");
        Console.WriteLine("║       CONVERSATION HISTORY             ║");
        Console.WriteLine("╚════════════════════════════════════════╝");
        Console.WriteLine();

        var task = Task.Run(async () =>
            await _apiClient.GetConversationHistoryAsync(_currentConversationId));
        var result = task.Result;

        if (!result.success || result.messages.Count == 0)
        {
            Console.WriteLine("No messages in this conversation.");
            Console.ReadKey();
            return;
        }

        foreach (var msg in result.messages)
        {
            Console.WriteLine($"{msg.Sender}: {msg.Content}");
            Console.WriteLine($"  [{msg.Timestamp:g}]");
            Console.WriteLine();
        }

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    private void SearchConversations()
    {
        Console.Clear();
        Console.WriteLine("╔════════════════════════════════════════╗");
        Console.WriteLine("║       SEARCH CONVERSATIONS             ║");
        Console.WriteLine("╚════════════════════════════════════════╝");
        Console.WriteLine();

        Console.Write("Search query: ");
        string? query = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(query))
        {
            Console.WriteLine("Search query cannot be empty.");
            Console.ReadKey();
            return;
        }

        var task = Task.Run(async () => await _apiClient.SearchConversationsAsync(query));
        var result = task.Result;

        if (!result.success)
        {
            Console.WriteLine("Search failed.");
            Console.ReadKey();
            return;
        }

        if (result.results.Count == 0)
        {
            Console.WriteLine("No results found.");
            Console.ReadKey();
            return;
        }

        Console.WriteLine($"\nFound {result.results.Count} result(s):");
        Console.WriteLine();

        foreach (var conv in result.results)
        {
            Console.WriteLine($"• {conv.Title} ({conv.MessageCount} messages)");
            Console.WriteLine($"  Created: {conv.CreatedAt:G}");
            Console.WriteLine();
        }

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    private void ViewAnalytics()
    {
        Console.Clear();
        Console.WriteLine("╔════════════════════════════════════════╗");
        Console.WriteLine("║          VIEW ANALYTICS                ║");
        Console.WriteLine("╚════════════════════════════════════════╝");
        Console.WriteLine();

        if (string.IsNullOrWhiteSpace(_currentConversationId))
        {
            Console.WriteLine("Please select a conversation first.");
            Console.ReadKey();
            return;
        }

        var task = Task.Run(async () =>
            await _apiClient.GetAnalyticsAsync(_currentConversationId));
        var result = task.Result;

        Console.WriteLine(result);
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }

    private void ManagePreferences()
    {
        Console.Clear();
        Console.WriteLine("╔════════════════════════════════════════╗");
        Console.WriteLine("║       MANAGE PREFERENCES               ║");
        Console.WriteLine("╚════════════════════════════════════════╝");
        Console.WriteLine();

        var task = Task.Run(async () => await _apiClient.GetPreferencesAsync());
        var result = task.Result;

        Console.WriteLine(result);
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }

    private void ViewNotifications()
    {
        Console.Clear();
        Console.WriteLine("╔════════════════════════════════════════╗");
        Console.WriteLine("║        YOUR NOTIFICATIONS              ║");
        Console.WriteLine("╚════════════════════════════════════════╝");
        Console.WriteLine();

        var task = Task.Run(async () => await _apiClient.GetNotificationsAsync());
        var result = task.Result;

        Console.WriteLine(result);
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }

    private void ExportConversation()
    {
        if (string.IsNullOrWhiteSpace(_currentConversationId))
        {
            Console.WriteLine("Please select a conversation first.");
            Console.ReadKey();
            return;
        }

        ExportCurrentConversation();
    }

    private void ExportCurrentConversation()
    {
        if (string.IsNullOrWhiteSpace(_currentConversationId))
        {
            Console.WriteLine("No conversation selected for export.");
            return;
        }

        Console.WriteLine("\nChoose export format:");
        Console.WriteLine("1. CSV");
        Console.WriteLine("2. JSON");
        Console.Write("Choice: ");

        string? choice = Console.ReadLine();
        string format = choice == "2" ? "json" : "csv";

        var task = Task.Run(async () =>
            await _apiClient.ExportConversationAsync(_currentConversationId, format));
        var result = task.Result;

        Console.WriteLine("\nExport result:");
        Console.WriteLine(result);
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }

    private void EnterpriseMenu()
    {
        Console.Clear();
        Console.WriteLine("╔════════════════════════════════════════╗");
        Console.WriteLine("║       ENTERPRISE FEATURES              ║");
        Console.WriteLine("╚════════════════════════════════════════╝");
        Console.WriteLine();
        Console.WriteLine("1. View Webhooks");
        Console.WriteLine("2. Two-Factor Authentication");
        Console.WriteLine("3. Manage API Keys");
        Console.WriteLine("4. IP Whitelist");
        Console.WriteLine("5. Generate Reports");
        Console.WriteLine("6. Back to Main Menu");
        Console.WriteLine();
        Console.Write("Choose an option: ");

        string? choice = Console.ReadLine();
        switch (choice)
        {
            case "1":
                ViewWebhooks();
                break;
            case "2":
                Console.WriteLine("\nTwo-Factor Authentication management coming soon...");
                Console.ReadKey();
                break;
            case "3":
                Console.WriteLine("\nAPI Key management coming soon...");
                Console.ReadKey();
                break;
            case "4":
                Console.WriteLine("\nIP Whitelist management coming soon...");
                Console.ReadKey();
                break;
            case "5":
                Console.WriteLine("\nReport generation coming soon...");
                Console.ReadKey();
                break;
        }
    }

    private void ViewWebhooks()
    {
        Console.Write("Enter API Key (or press Enter to skip): ");
        string? apiKey = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            Console.WriteLine("API key required for webhooks.");
            Console.ReadKey();
            return;
        }

        var task = Task.Run(async () => await _apiClient.GetWebhooksAsync(apiKey));
        var result = task.Result;

        Console.WriteLine("\nWebhooks:");
        Console.WriteLine(result);
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }

    private void TestApiHealth()
    {
        Console.WriteLine("\nTesting API health...");
        var task = Task.Run(async () => await _apiClient.HealthCheckAsync());
        bool isHealthy = task.Result;

        if (isHealthy)
        {
            Console.WriteLine("✓ API is healthy and responding!");
        }
        else
        {
            Console.WriteLine("✗ API is not responding. Please check the connection.");
        }

        Console.ReadKey();
    }

    private void RunLocalChatbot()
    {
        var chatBot = new ChatBot("Assistant");

        Console.WriteLine("Local Chatbot Mode - Type 'exit' to return to menu");
        Console.WriteLine();

        bool running = true;
        while (running)
        {
            Console.Write("You: ");
            string? input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
                continue;

            if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
            {
                running = false;
                break;
            }

            string response = chatBot.ProcessMessage(input);
            Console.WriteLine($"Assistant: {response}\n");
        }
    }

    private static string ReadPassword()
    {
        string password = string.Empty;
        ConsoleKeyInfo key;

        do
        {
            key = Console.ReadKey(true);

            if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
            {
                password += key.KeyChar;
                Console.Write("*");
            }
            else if (key.Key == ConsoleKey.Backspace)
            {
                if (password.Length > 0)
                {
                    password = password.Substring(0, password.Length - 1);
                    Console.Write("\b \b");
                }
            }
        } while (key.Key != ConsoleKey.Enter);

        return password;
    }
}
