namespace Chatbot.Core.Models;

public class IntentDistribution
{
    public string Intent { get; set; } = string.Empty;
    public int Count { get; set; }
    public double Percentage { get; set; }
}
