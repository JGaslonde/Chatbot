using Chatbot.Core.Models.Responses;
using Chatbot.API.Data.Repositories.Interfaces;
using Chatbot.API.Services.Analytics.Interfaces;
using Microsoft.Extensions.Logging;

namespace Chatbot.API.Services.Analytics;

public class AdvancedAnalyticsService : IAdvancedAnalyticsService
{
    private readonly IConversationRepository _conversationRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly ILogger<AdvancedAnalyticsService> _logger;

    public AdvancedAnalyticsService(
        IConversationRepository conversationRepository,
        IMessageRepository messageRepository,
        ILogger<AdvancedAnalyticsService> logger)
    {
        _conversationRepository = conversationRepository;
        _messageRepository = messageRepository;
        _logger = logger;
    }

    public async Task<AdvancedAnalyticsResponse> GetAdvancedAnalyticsAsync(
        int userId,
        DateTime fromDate,
        DateTime toDate)
    {
        try
        {
            var conversations = (await _conversationRepository.GetUserConversationsAsync(userId)).ToList();
            var filteredConversations = conversations
                .Where(c => c.StartedAt >= fromDate && c.StartedAt <= toDate)
                .ToList();

            var allMessages = new List<Chatbot.Core.Models.Entities.Message>();
            foreach (var conversation in filteredConversations)
            {
                var msgs = await _messageRepository.GetConversationMessagesAsync(conversation.Id);
                allMessages.AddRange(msgs);
            }

            var avgSentiment = allMessages.Count > 0 ? allMessages.Average(m => m.SentimentScore) : 0;
            var intentDistribution = GetIntentDistribution(allMessages);
            var dailyMessageCount = GetDailyMessageCount(allMessages);

            var avgConversationDuration = filteredConversations.Count > 0
                ? filteredConversations.Average(c => 
                    (c.LastMessageAt.Subtract(c.StartedAt).TotalMinutes))
                : 0;

            return new AdvancedAnalyticsResponse(
                filteredConversations.Count,
                allMessages.Count,
                avgSentiment,
                intentDistribution,
                dailyMessageCount,
                avgConversationDuration,
                fromDate,
                toDate
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating advanced analytics for user {UserId}", userId);
            throw;
        }
    }

    public async Task<Dictionary<string, int>> GetMessageTrendsByDayAsync(
        int userId,
        DateTime fromDate,
        DateTime toDate)
    {
        try
        {
            var conversations = (await _conversationRepository.GetUserConversationsAsync(userId)).ToList();
            var allMessages = new List<Chatbot.Core.Models.Entities.Message>();
            foreach (var conversation in conversations)
            {
                var msgs = await _messageRepository.GetConversationMessagesAsync(conversation.Id);
                allMessages.AddRange(msgs);
            }

            var messages = allMessages
                .Where(m => m.SentAt >= fromDate && m.SentAt <= toDate)
                .ToList();

            return GetDailyMessageCount(messages);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting message trends for user {UserId}", userId);
            throw;
        }
    }

    public async Task<Dictionary<string, int>> GetIntentDistributionAsync(int userId)
    {
        try
        {
            var conversations = (await _conversationRepository.GetUserConversationsAsync(userId)).ToList();
            var allMessages = new List<Chatbot.Core.Models.Entities.Message>();
            foreach (var conversation in conversations)
            {
                var msgs = await _messageRepository.GetConversationMessagesAsync(conversation.Id);
                allMessages.AddRange(msgs);
            }

            return GetIntentDistribution(allMessages);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting intent distribution for user {UserId}", userId);
            throw;
        }
    }

    private Dictionary<string, int> GetIntentDistribution(List<Chatbot.Core.Models.Entities.Message> messages)
    {
        return messages
            .GroupBy(m => m.DetectedIntent ?? "Unknown")
            .ToDictionary(g => g.Key, g => g.Count());
    }

    private Dictionary<string, int> GetDailyMessageCount(List<Chatbot.Core.Models.Entities.Message> messages)
    {
        return messages
            .GroupBy(m => m.SentAt.Date.ToString("yyyy-MM-dd"))
            .ToDictionary(g => g.Key, g => g.Count());
    }
}
