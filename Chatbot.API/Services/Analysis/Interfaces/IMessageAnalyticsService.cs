namespace Chatbot.API.Services.Analysis.Interfaces;

public interface IMessageAnalyticsService
{
    Task<MessageAnalysisResult> AnalyzeMessageAsync(string content);
}
