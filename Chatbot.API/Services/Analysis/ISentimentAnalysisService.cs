using Chatbot.Core.Models.Entities;

namespace Chatbot.API.Services.Analysis;

public interface ISentimentAnalysisService
{
    Task<(Sentiment Sentiment, double Score)> AnalyzeSentimentAsync(string text);
}
