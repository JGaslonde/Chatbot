using Chatbot.Core.Models.Entities;

namespace Chatbot.API.Services.Analysis.Interfaces;

public interface ISentimentAnalysisService
{
    Task<(Sentiment Sentiment, double Score)> AnalyzeSentimentAsync(string text);
}
