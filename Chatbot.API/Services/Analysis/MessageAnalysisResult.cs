using Chatbot.Core.Models.Entities;

namespace Chatbot.API.Services.Analysis;

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
