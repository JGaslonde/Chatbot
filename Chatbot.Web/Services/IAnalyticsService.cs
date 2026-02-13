using Chatbot.Web.Models;

namespace Chatbot.Web.Services;

public interface IAnalyticsService
{
    Task<(bool Success, string Message, ConversationAnalytics? Analytics)> GetAnalyticsAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<(bool Success, string Message, List<SentimentTrend>? Trends)> GetSentimentTrendsAsync(int days = 7);
    Task<(bool Success, string Message, List<IntentDistribution>? Distribution)> GetIntentDistributionAsync(int days = 30);
}
