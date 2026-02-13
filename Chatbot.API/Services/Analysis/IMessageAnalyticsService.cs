namespace Chatbot.API.Services.Analysis;

public interface IMessageAnalyticsService
{
    Task<MessageAnalysisResult> AnalyzeMessageAsync(string content);
}
