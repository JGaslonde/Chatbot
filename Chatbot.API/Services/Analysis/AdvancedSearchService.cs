using Chatbot.Core.Models.Entities;
using Chatbot.Core.Models.Requests;
using Chatbot.Core.Models.Responses;
using Chatbot.API.Data.Repositories.Interfaces;
using Chatbot.API.Services.Analysis.Interfaces;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Chatbot.API.Services.Analysis;

public class AdvancedSearchService : IAdvancedSearchService
{
    private readonly IConversationRepository _conversationRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly ILogger<AdvancedSearchService> _logger;

    public AdvancedSearchService(
        IConversationRepository conversationRepository,
        IMessageRepository messageRepository,
        ILogger<AdvancedSearchService> logger)
    {
        _conversationRepository = conversationRepository;
        _messageRepository = messageRepository;
        _logger = logger;
    }

    public async Task<SearchResultsResponse> SearchConversationsAsync(
        int userId,
        SearchConversationsRequest request)
    {
        try
        {
            var conversations = (await _conversationRepository.GetUserConversationsAsync(userId)).ToList();
            
            // Filter by query if provided
            if (!string.IsNullOrWhiteSpace(request.Query))
            {
                var lowerQuery = request.Query.ToLower();
                conversations = conversations
                    .Where(c => c.Title.ToLower().Contains(lowerQuery) ||
                               (c.Summary?.ToLower().Contains(lowerQuery) ?? false))
                    .ToList();
            }

            // Filter by date range if provided
            if (request.StartDate.HasValue && request.EndDate.HasValue)
            {
                conversations = conversations
                    .Where(c => c.StartedAt >= request.StartDate &&
                               c.StartedAt <= request.EndDate)
                    .ToList();
            }

            // Sort conversations
            conversations = SortConversations(conversations, request.SortBy, request.AscendingOrder);

            // Paginate
            var totalCount = conversations.Count;
            var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);
            var skip = (request.Page - 1) * request.PageSize;
            var pagedResults = conversations.Skip(skip).Take(request.PageSize).ToList();

            var results = pagedResults.Select(c => new ConversationResponse(
                c.Id,
                c.Title,
                c.StartedAt,
                c.Messages?.Count ?? 0,
                c.Summary
            )).ToList();

            return new SearchResultsResponse(
                results,
                totalCount,
                request.Page,
                request.PageSize,
                totalPages
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching conversations for user {UserId}", userId);
            throw;
        }
    }

    public async Task<SearchResultsResponse> SearchMessagesAsync(
        int userId,
        string query,
        int page = 1,
        int pageSize = 20)
    {
        try
        {
            var conversations = (await _conversationRepository.GetUserConversationsAsync(userId)).ToList();
            var messages = new List<Message>();

            foreach (var conversation in conversations)
            {
                var conversationMessages = (await _messageRepository.GetConversationMessagesAsync(conversation.Id)).ToList();
                messages.AddRange(conversationMessages);
            }

            if (!string.IsNullOrWhiteSpace(query))
            {
                var lowerQuery = query.ToLower();
                messages = messages
                    .Where(m => m.Content.ToLower().Contains(lowerQuery))
                    .ToList();
            }

            var totalCount = messages.Count;
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            var skip = (page - 1) * pageSize;
            var pagedMessages = messages.Skip(skip).Take(pageSize).ToList();

            var results = pagedMessages.Select(m => new MessageDto(
                m.Id,
                m.Content,
                m.Sender.ToString(),
                m.SentAt,
                m.Sentiment.ToString(),
                m.DetectedIntent,
                m.SentimentScore
            )).ToList();

            // Return as empty conversation list for now - could create specialized response type
            return new SearchResultsResponse(
                new List<ConversationResponse>(),
                totalCount,
                page,
                pageSize,
                totalPages
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching messages for user {UserId}", userId);
            throw;
        }
    }

    private List<Conversation> SortConversations(
        List<Conversation> conversations,
        string? sortBy,
        bool ascending)
    {
        var sorted = sortBy?.ToLower() switch
        {
            "title" => conversations.OrderBy(c => c.Title),
            "date" => conversations.OrderBy(c => c.StartedAt),
            "messages" => conversations.OrderBy(c => c.Messages?.Count ?? 0),
            _ => conversations.OrderBy(c => c.LastMessageAt)
        };

        return ascending ? sorted.ToList() : sorted.Reverse().ToList();
    }
}
