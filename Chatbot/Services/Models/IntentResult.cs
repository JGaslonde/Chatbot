namespace Chatbot.Services.Models;

/// <summary>
/// Detected user intent
/// </summary>
public class IntentResult
{
    public string Intent { get; set; } = "unknown";
    public double Confidence { get; set; } // 0.0 to 1.0
}
