using Chatbot.Core.Models.Entities;
using Chatbot.Core.Models.Requests;
using Chatbot.Core.Models.Responses;
using Chatbot.API.Data.Repositories;
using Chatbot.API.Data.Repositories.Interfaces;
using Chatbot.API.Services.Phase3.Interfaces;
using Microsoft.Extensions.Logging;

namespace Chatbot.API.Services.Phase3;

public class ConversationAnalyticsService : IConversationAnalyticsService
{
    private readonly IConversationAnalyticsRepository _repository;
    private readonly IConversationRepository _conversationRepository;
    private readonly IRepository<Message> _messageRepository;
    private readonly ILogger<ConversationAnalyticsService> _logger;

    public ConversationAnalyticsService(
        IConversationAnalyticsRepository repository,
        IConversationRepository conversationRepository,
        IRepository<Message> messageRepository,
        ILogger<ConversationAnalyticsService> logger)
    {
        _repository = repository;
        _conversationRepository = conversationRepository;
        _messageRepository = messageRepository;
        _logger = logger;
    }

    public async Task<ConversationAnalyticsDto?> GetAnalyticsAsync(int conversationId)
    {
        var analytics = await _repository.GetByConversationIdAsync(conversationId);
        return analytics == null ? null : MapToDto(analytics);
    }

    public async Task<List<ConversationAnalyticsDto>> GetUserAnalyticsByDateRangeAsync(int userId, DateTime startDate, DateTime endDate)
    {
        var analytics = await _repository.GetUserAnalyticsByDateRangeAsync(userId, startDate, endDate);
        return analytics.Select(MapToDto).ToList();
    }

    public async Task<double> GetAverageSentimentAsync(int userId, int days = 30)
    {
        return await _repository.GetAverageSentimentAsync(userId, days);
    }

    public async Task<AnalyticsSummaryDto> GetAnalyticsSummaryAsync(int userId, int days = 30)
    {
        var since = DateTime.UtcNow.AddDays(-days);
        var analytics = await _repository.GetUserAnalyticsByDateRangeAsync(userId, since, DateTime.UtcNow);

        if (analytics.Count == 0)
            return new AnalyticsSummaryDto(
                DateRange: DateTime.UtcNow,
                TotalConversations: 0,
                AverageEngagementScore: 0,
                AverageSentimentScore: 0.5,
                TotalMessages: 0,
                AverageResponseTime: 0,
                TopTopics: new List<TopicFrequencyDto>(),
                TopIntents: new List<IntentFrequencyDto>(),
                SentimentDistribution: new Dictionary<string, int>(),
                KeyInsights: new List<MLInsightDto>()
            );

        var topTopics = ExtractTopTopics(analytics);
        var topIntents = ExtractTopIntents(analytics);
        var sentimentCounts = CalculateSentimentDistribution(analytics);

        return new AnalyticsSummaryDto(
            DateRange: DateTime.UtcNow,
            TotalConversations: analytics.Count,
            AverageEngagementScore: analytics.Average(a => a.EngagementScore),
            AverageSentimentScore: analytics.Average(a => a.AverageSentimentScore),
            TotalMessages: analytics.Sum(a => a.TotalMessages),
            AverageResponseTime: analytics.Average(a => a.AverageResponseTime),
            TopTopics: topTopics,
            TopIntents: topIntents,
            SentimentDistribution: sentimentCounts,
            KeyInsights: new List<MLInsightDto>() // Will be populated from ML insights service
        );
    }

    public async Task<ConversationAnalyticsEntity> CreateOrUpdateAnalyticsAsync(
        int conversationId, int userId, AnalyticsRequest request)
    {
        var existing = await _repository.GetByConversationIdAsync(conversationId);

        if (existing != null)
        {
            existing.UpdatedAt = DateTime.UtcNow;
            await _repository.UpdateAsync(existing);
            return existing;
        }

        var analytics = new ConversationAnalyticsEntity
        {
            UserId = userId,
            ConversationId = conversationId,
            AverageSentimentScore = 0.5,
            TotalMessages = 0,
            UserMessageCount = 0,
            BotMessageCount = 0,
            AverageResponseTime = 0,
            AverageMessageLength = 0,
            DominantIntent = null,
            DetectedTopics = null,
            EngagementScore = 50,
            IsSatisfactory = null,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(analytics);
        return analytics;
    }

    private List<TopicFrequencyDto> ExtractTopTopics(List<ConversationAnalyticsEntity> analytics, int top = 5)
    {
        var topicCounts = new Dictionary<string, int>();
        foreach (var a in analytics)
        {
            if (!string.IsNullOrEmpty(a.DetectedTopics))
            {
                foreach (var topic in a.DetectedTopics.Split(','))
                {
                    var trimmed = topic.Trim();
                    if (topicCounts.ContainsKey(trimmed))
                        topicCounts[trimmed]++;
                    else
                        topicCounts[trimmed] = 1;
                }
            }
        }

        return topicCounts
            .OrderByDescending(x => x.Value)
            .Take(top)
            .Select(x => new TopicFrequencyDto(x.Key, x.Value, (x.Value * 100.0) / analytics.Count))
            .ToList();
    }

    private List<IntentFrequencyDto> ExtractTopIntents(List<ConversationAnalyticsEntity> analytics, int top = 5)
    {
        var intentCounts = new Dictionary<string, (int count, double totalSentiment)>();
        foreach (var a in analytics)
        {
            if (!string.IsNullOrEmpty(a.DominantIntent))
            {
                if (intentCounts.ContainsKey(a.DominantIntent))
                {
                    var (count, sentiment) = intentCounts[a.DominantIntent];
                    intentCounts[a.DominantIntent] = (count + 1, sentiment + a.AverageSentimentScore);
                }
                else
                    intentCounts[a.DominantIntent] = (1, a.AverageSentimentScore);
            }
        }

        return intentCounts
            .OrderByDescending(x => x.Value.count)
            .Take(top)
            .Select(x => new IntentFrequencyDto(
                x.Key, x.Value.count, (x.Value.count * 100.0) / analytics.Count, x.Value.totalSentiment / x.Value.count))
            .ToList();
    }

    private Dictionary<string, int> CalculateSentimentDistribution(List<ConversationAnalyticsEntity> analytics)
    {
        var positive = analytics.Count(a => a.AverageSentimentScore >= 0.6);
        var neutral = analytics.Count(a => a.AverageSentimentScore >= 0.4 && a.AverageSentimentScore < 0.6);
        var negative = analytics.Count(a => a.AverageSentimentScore < 0.4);

        return new Dictionary<string, int>
        {
            { "Positive", positive },
            { "Neutral", neutral },
            { "Negative", negative }
        };
    }

    private ConversationAnalyticsDto MapToDto(ConversationAnalyticsEntity entity)
    {
        return new ConversationAnalyticsDto(
            Id: entity.Id,
            ConversationId: entity.ConversationId,
            AverageSentimentScore: entity.AverageSentimentScore,
            TotalMessages: entity.TotalMessages,
            UserMessageCount: entity.UserMessageCount,
            BotMessageCount: entity.BotMessageCount,
            AverageResponseTime: entity.AverageResponseTime,
            AverageMessageLength: entity.AverageMessageLength,
            DominantIntent: entity.DominantIntent,
            DetectedTopics: entity.DetectedTopics,
            EngagementScore: entity.EngagementScore,
            IsSatisfactory: entity.IsSatisfactory,
            UpdatedAt: entity.UpdatedAt
        );
    }
}
