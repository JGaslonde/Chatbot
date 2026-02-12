using Chatbot.API.Data;
using Chatbot.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Chatbot.API.Services;

public interface IConversationAnalyticsService
{
    Task<ConversationAnalytics> GetAnalyticsAsync(int? userId = null, DateTime? startDate = null, DateTime? endDate = null);
    Task<List<SentimentTrend>> GetSentimentTrendsAsync(int userId, int days = 7);
    Task<List<IntentDistribution>> GetIntentDistributionAsync(int userId, int days = 30);
}

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

        // Active conversations
        var conversationsQuery = (await _conversationRepository.GetAllAsync())
            .Where(c => c.LastMessageAt >= start && c.LastMessageAt <= end);

        if (userId.HasValue)
        {
            conversationsQuery = conversationsQuery.Where(c => c.UserId == userId.Value);
        }

        var activeConversations = conversationsQuery.Count();

        // Active users
        var activeUsers = userId.HasValue
            ? 1
            : (await _conversationRepository.GetAllAsync())
                .Where(c => c.LastMessageAt >= start && c.LastMessageAt <= end)
                .Select(c => c.UserId)
                .Distinct()
                .Count();

        return new ConversationAnalytics
        {
            StartDate = start,
            EndDate = end,
            TotalMessages = totalMessages,
            UserMessages = userMessages,
            BotMessages = botMessages,
            AverageSentiment = Math.Round(avgSentiment, 2),
            SentimentDistribution = sentimentDist,
            IntentDistribution = intentDist,
            ActiveConversations = activeConversations,
            ActiveUsers = activeUsers
        };
    }

    public async Task<List<SentimentTrend>> GetSentimentTrendsAsync(int userId, int days = 7)
    {
        var startDate = DateTime.UtcNow.AddDays(-days);
        var userConversationIds = (await _conversationRepository.GetAllAsync())
            .Where(c => c.UserId == userId)
            .Select(c => c.Id)
            .ToList();

        var messages = (await _messageRepository.GetAllAsync())
            .Where(m => userConversationIds.Contains(m.ConversationId)
                     && m.Sender == MessageSender.User
                     && m.SentAt >= startDate)
            .ToList();

        var trends = messages
            .GroupBy(m => m.SentAt.Date)
            .Select(g => new SentimentTrend
            {
                Date = g.Key,
                AverageSentiment = Math.Round(g.Average(m => m.SentimentScore), 2),
                MessageCount = g.Count()
            })
            .OrderBy(t => t.Date)
            .ToList();

        return trends;
    }

    public async Task<List<IntentDistribution>> GetIntentDistributionAsync(int userId, int days = 30)
    {
        var startDate = DateTime.UtcNow.AddDays(-days);
        var userConversationIds = (await _conversationRepository.GetAllAsync())
            .Where(c => c.UserId == userId)
            .Select(c => c.Id)
            .ToList();

        var messages = (await _messageRepository.GetAllAsync())
            .Where(m => userConversationIds.Contains(m.ConversationId)
                     && m.Sender == MessageSender.User
                     && m.SentAt >= startDate
                     && !string.IsNullOrEmpty(m.DetectedIntent))
            .ToList();

        var distribution = messages
            .GroupBy(m => m.DetectedIntent)
            .Select(g => new IntentDistribution
            {
                Intent = g.Key ?? "unknown",
                Count = g.Count(),
                Percentage = messages.Count > 0 ? Math.Round((double)g.Count() / messages.Count * 100, 1) : 0
            })
            .OrderByDescending(i => i.Count)
            .ToList();

        return distribution;
    }
}

public class ConversationAnalytics
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalMessages { get; set; }
    public int UserMessages { get; set; }
    public int BotMessages { get; set; }
    public double AverageSentiment { get; set; }
    public Dictionary<string, int> SentimentDistribution { get; set; } = new();
    public Dictionary<string, int> IntentDistribution { get; set; } = new();
    public int ActiveConversations { get; set; }
    public int ActiveUsers { get; set; }
}

public class SentimentTrend
{
    public DateTime Date { get; set; }
    public double AverageSentiment { get; set; }
    public int MessageCount { get; set; }
}

public class IntentDistribution
{
    public string Intent { get; set; } = string.Empty;
    public int Count { get; set; }
    public double Percentage { get; set; }
}
