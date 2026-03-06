using Chatbot;
using Chatbot.Services;

Console.Clear();
Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
Console.WriteLine("║                                                            ║");
Console.WriteLine("║         Welcome to Advanced C# Chatbot Console             ║");
Console.WriteLine("║                                                            ║");
Console.WriteLine("║  Features: Remote API Integration • Authentication        ║");
Console.WriteLine("║            Analytics • Search • Export • Enterprise APIs  ║");
Console.WriteLine("║                                                            ║");
Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
Console.WriteLine();

// Initialize API client (will attempt to connect to localhost:5089)
Console.WriteLine("Initializing API client...");
var apiClient = new ApiClient("http://localhost:5089");

// Check API availability
var healthCheck = Task.Run(async () => await apiClient.HealthCheckAsync()).Result;

if (!healthCheck)
{
    Console.WriteLine("\n⚠ WARNING: API server not responding at http://localhost:5089");
    Console.WriteLine("The console will operate in LOCAL MODE with limited features.");
    Console.WriteLine("\nTo use full features, ensure the API is running:");
    Console.WriteLine("  cd Chatbot.API && dotnet run");
    Console.WriteLine("\nPress any key to continue...");
    Console.ReadKey();
}
else
{
    Console.WriteLine("✓ API server connected successfully!\n");
}

System.Threading.Thread.Sleep(1500);

// Launch the menu system
var menu = new ConsoleMenu(apiClient);
menu.Run();

