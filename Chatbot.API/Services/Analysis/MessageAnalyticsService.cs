using Chatbot.API.Services.Processing.Interfaces;
using Microsoft.Extensions.Logging;
using Chatbot.API.Services.Analysis.Interfaces;

namespace Chatbot.API.Services.Analysis;

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
