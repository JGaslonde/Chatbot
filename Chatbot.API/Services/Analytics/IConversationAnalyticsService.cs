using Chatbot.API.Data.Repositories;
using Chatbot.Core.Models;
using Chatbot.Core.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Chatbot.API.Services.Analytics;

public interface IConversationAnalyticsService
{
    Task<ConversationAnalytics> GetAnalyticsAsync(int? userId = null, DateTime? startDate = null, DateTime? endDate = null);
    Task<List<SentimentTrend>> GetSentimentTrendsAsync(int userId, int days = 7);
    Task<List<IntentDistribution>> GetIntentDistributionAsync(int userId, int days = 30);
}
