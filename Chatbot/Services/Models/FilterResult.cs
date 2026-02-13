namespace Chatbot.Services.Models;

/// <summary>
/// Result of message filtering
/// </summary>
public class FilterResult
{
    public bool IsFiltered { get; set; }
    public List<string> Reasons { get; set; } = new();
}
