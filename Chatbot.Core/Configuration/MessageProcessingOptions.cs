namespace Chatbot.Core.Configuration;

/// <summary>
/// Configuration options for message processing.
/// Applies Single Responsibility - Isolates configuration concerns.
/// Follows Open/Closed Principle - Can be extended without modifying other code.
/// </summary>
public class MessageProcessingOptions
{
    public const string SectionName = "MessageProcessing";

    /// <summary>
    /// How often to update conversation summaries (e.g., every nth message).
    /// </summary>
    public int SummaryUpdateFrequency { get; set; } = 5;

    /// <summary>
    /// Maximum number of recent messages to consider for context.
    /// </summary>
    public int ContextMessageCount { get; set; } = 10;

    /// <summary>
    /// Whether to enable message filtering.
    /// </summary>
    public bool EnableMessageFiltering { get; set; } = true;

    /// <summary>
    /// Whether to enable sentiment analysis.
    /// </summary>
    public bool EnableSentimentAnalysis { get; set; } = true;

    /// <summary>
    /// Minimum sentiment score threshold for filtering.
    /// </summary>
    public double SentimentScoreThreshold { get; set; } = 0.5;
}
