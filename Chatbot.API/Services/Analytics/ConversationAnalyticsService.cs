using Chatbot.API.Data.Repositories;
using Chatbot.Core.Models;
using Chatbot.Core.Models.Entities;
using Chatbot.Core.Models.Analytics;
using Chatbot.API.Services.Analytics.Interfaces;

namespace Chatbot.API.Services.Analytics;

public class ConversationAnalyticsService : IConversationAnalyticsService
{
    private readonly Repository<Message> _messageRepository;
    private readonly Repository<Conversation> _conversationRepository;
    private readonly Repository<User> _userRepository;

    public ConversationAnalyticsService(
        Repository<Message> messageRepository,
        Repository<Conversation> conversationRepository,
        Repository<User> userRepository)
    {
        _messageRepository = messageRepository;
        _conversationRepository = conversationRepository;
        _userRepository = userRepository;
    }

    public async Task<ConversationAnalytics> GetAnalyticsAsync(int? userId = null, DateTime? startDate = null, DateTime? endDate = null)
    {
        var start = startDate ?? DateTime.UtcNow.AddDays(-30);
        var end = endDate ?? DateTime.UtcNow;

        var messagesQuery = (await _messageRepository.GetAllAsync())
            .Where(m => m.SentAt >= start && m.SentAt <= end);

        // Filter by user if provided
        if (userId.HasValue)
        {
            var userConversationIds = (await _conversationRepository.GetAllAsync())
                .Where(c => c.UserId == userId.Value)
                .Select(c => c.Id)
                .ToList();
            messagesQuery = messagesQuery.Where(m => userConversationIds.Contains(m.ConversationId));
        }

        var messages = messagesQuery.ToList();
        var totalMessages = messages.Count;
        var userMessages = messages.Count(m => m.Sender == MessageSender.User);
        var botMessages = totalMessages - userMessages;

        // Calculate average sentiment
        var sentimentMessages = messages.Where(m => m.Sender == MessageSender.User).ToList();
        var avgSentiment = sentimentMessages.Any()
            ? sentimentMessages.Average(m => m.SentimentScore)
            : 0;

        // Sentiment distribution
        var sentimentDist = new Dictionary<string, int>
        {
            ["VeryPositive"] = sentimentMessages.Count(m => m.Sentiment == Sentiment.VeryPositive),
            ["Positive"] = sentimentMessages.Count(m => m.Sentiment == Sentiment.Positive),
            ["Neutral"] = sentimentMessages.Count(m => m.Sentiment == Sentiment.Neutral),
            ["Negative"] = sentimentMessages.Count(m => m.Sentiment == Sentiment.Negative),
            ["VeryNegative"] = sentimentMessages.Count(m => m.Sentiment == Sentiment.VeryNegative)
        };

        // Intent distribution
        var intentMessages = messages.Where(m => !string.IsNullOrEmpty(m.DetectedIntent) && m.Sender == MessageSender.User).ToList();
        var intentDist = intentMessages
            .GroupBy(m => m.DetectedIntent)
            .ToDictionary(g => g.Key ?? "unknown", g => g.Count());

        return new ConversationAnalytics
        {
            TotalMessages = totalMessages,
            UserMessageCount = userMessages,
            BotMessageCount = botMessages,
            AverageSentimentScore = avgSentiment,
            SentimentDistribution = sentimentDist,
            IntentDistribution = intentDist,
            DateRange = new DateRange { Start = start, End = end }
        };
    }

    public async Task<List<SentimentTrend>> GetSentimentTrendsAsync(int userId, int days = 7)
    {
        var userConversationIds = (await _conversationRepository.GetAllAsync())
            .Where(c => c.UserId == userId)
            .Select(c => c.Id)
            .ToList();

        var startDate = DateTime.UtcNow.AddDays(-days);
        var messages = (await _messageRepository.GetAllAsync())
            .Where(m => m.SentAt >= startDate && userConversationIds.Contains(m.ConversationId) && m.Sender == MessageSender.User)
            .ToList();

        var trends = messages
            .GroupBy(m => m.SentAt.Date)
            .OrderBy(g => g.Key)
            .Select(g => new SentimentTrend
            {
                Date = g.Key,
                AvergeSentiment = g.Average(m => m.SentimentScore),
                MessageCount = g.Count()
            })
            .ToList();

        return trends;
    }

    public async Task<List<IntentDistribution>> GetIntentDistributionAsync(int userId, int days = 30)
    {
        var userConversationIds = (await _conversationRepository.GetAllAsync())
            .Where(c => c.UserId == userId)
            .Select(c => c.Id)
            .ToList();

        var startDate = DateTime.UtcNow.AddDays(-days);
        var messages = (await _messageRepository.GetAllAsync())
            .Where(m => m.SentAt >= startDate && userConversationIds.Contains(m.ConversationId) && m.Sender == MessageSender.User)
            .ToList();

        var distribution = messages
            .Where(m => !string.IsNullOrEmpty(m.DetectedIntent))
            .GroupBy(m => m.DetectedIntent)
            .Select(g => new IntentDistribution
            {
                Intent = g.Key ?? "unknown",
                Count = g.Count(),
                Percentage = (g.Count() / (double)messages.Count) * 100
            })
            .OrderByDescending(x => x.Count)
            .ToList();

        return distribution;
    }
}
