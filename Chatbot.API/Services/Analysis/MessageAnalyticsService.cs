using Chatbot.Core.Models.Entities;
using Chatbot.API.Services.Processing;
using Microsoft.Extensions.Logging;

namespace Chatbot.API.Services.Analysis;

/// <summary>
/// Handles message analysis operations (sentiment, intent, filtering).
/// Implements Single Responsibility Principle - Focus only on message analysis.
/// Applies DRY - Consolidates message analysis logic used across the application.
/// </summary>
public interface IMessageAnalyticsService
{
    /// <summary>
    /// Analyzes a message for sentiment and intent, applies filtering.
    /// Returns analyzed message with all metadata.
    /// </summary>
    Task<MessageAnalysisResult> AnalyzeMessageAsync(string content);
}

/// <summary>
/// Result of message analysis containing all extracted data.
/// </summary>
public class MessageAnalysisResult
{
    public string CleanContent { get; set; } = string.Empty;
    public bool IsFiltered { get; set; }
    public string? FilterReason { get; set; }
    public Sentiment Sentiment { get; set; }
    public double SentimentScore { get; set; }
    public string? Intent { get; set; }
    public double IntentConfidence { get; set; }
}

public class MessageAnalyticsService : IMessageAnalyticsService
{
    private readonly ISentimentAnalysisService _sentimentService;
    private readonly IIntentRecognitionService _intentService;
    private readonly IMessageFilterService _filterService;
    private readonly ILogger<MessageAnalyticsService> _logger;

    public MessageAnalyticsService(
        ISentimentAnalysisService sentimentService,
        IIntentRecognitionService intentService,
        IMessageFilterService filterService,
        ILogger<MessageAnalyticsService> logger)
    {
        _sentimentService = sentimentService;
        _intentService = intentService;
        _filterService = filterService;
        _logger = logger;
    }

    public async Task<MessageAnalysisResult> AnalyzeMessageAsync(string content)
    {
        _logger.LogInformation("Analyzing message. ContentLength: {ContentLength}", content.Length);

        // Apply filtering
        var (isClean, issues) = await _filterService.FilterMessageAsync(content);

        // Analyze sentiment
        var (sentiment, sentimentScore) = await _sentimentService.AnalyzeSentimentAsync(content);

        // Recognize intent
        var (intent, intentConfidence) = await _intentService.RecognizeIntentAsync(content);

        var result = new MessageAnalysisResult
        {
            CleanContent = isClean ? content : "[Filtered content]",
            IsFiltered = !isClean,
            FilterReason = issues.Count > 0 ? string.Join("; ", issues) : null,
            Sentiment = sentiment,
            SentimentScore = sentimentScore,
            Intent = intent,
            IntentConfidence = intentConfidence
        };

        _logger.LogInformation("Message analysis complete. Sentiment: {Sentiment}, Intent: {Intent}, IsFiltered: {IsFiltered}",
            sentiment, intent, isClean);

        return result;
    }
}
