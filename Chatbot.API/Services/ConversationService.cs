using Chatbot.Core.Models.Entities;
using Chatbot.API.Data;

namespace Chatbot.API.Services;

public interface IConversationService
{
    Task<Conversation> CreateConversationAsync(int userId, string? title = null);
    Task<Conversation?> GetConversationAsync(int conversationId);
    Task<IEnumerable<Conversation>> GetUserConversationsAsync(int userId);
    Task<Message> AddMessageAsync(int conversationId, string content, MessageSender sender);
    Task<IEnumerable<Message>> GetConversationHistoryAsync(int conversationId);
    Task<bool> UpdateConversationAsync(int conversationId, string? title = null);
    Task<string> GenerateBotResponseAsync(int conversationId, string userMessage);
    Task UpdateConversationSummaryAsync(int conversationId);
}

public class ConversationService : IConversationService
{
    private readonly IUserRepository _userRepository;
    private readonly IConversationRepository _conversationRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly IMessageAnalyticsService _messageAnalytics;
    private readonly IResponseTemplateService _responseTemplateService;
    private readonly IConversationSummarizationService _summarizationService;
    private readonly ISentimentAnalysisService _sentimentService;
    private readonly IIntentRecognitionService _intentService;
    private readonly ILogger<ConversationService> _logger;

    public ConversationService(
        IUserRepository userRepository,
        IConversationRepository conversationRepository,
        IMessageRepository messageRepository,
        IMessageAnalyticsService messageAnalytics,
        IResponseTemplateService responseTemplateService,
        IConversationSummarizationService summarizationService,
        ISentimentAnalysisService sentimentService,
        IIntentRecognitionService intentService,
        ILogger<ConversationService> logger)
    {
        _userRepository = userRepository;
        _conversationRepository = conversationRepository;
        _messageRepository = messageRepository;
        _messageAnalytics = messageAnalytics;
        _responseTemplateService = responseTemplateService;
        _summarizationService = summarizationService;
        _sentimentService = sentimentService;
        _intentService = intentService;
        _logger = logger;
    }

    public async Task<Conversation> CreateConversationAsync(int userId, string? title = null)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new InvalidOperationException("User not found");

        var conversation = new Conversation
        {
            UserId = userId,
            Title = title ?? $"Conversation {DateTime.UtcNow:yyyy-MM-dd HH:mm}",
            StartedAt = DateTime.UtcNow,
            LastMessageAt = DateTime.UtcNow,
            IsActive = true,
            User = null!
        };

        return await _conversationRepository.AddAsync(conversation);
    }

    public async Task<Conversation?> GetConversationAsync(int conversationId)
    {
        return await _conversationRepository.GetWithMessagesAsync(conversationId);
    }

    public async Task<IEnumerable<Conversation>> GetUserConversationsAsync(int userId)
    {
        return await _conversationRepository.GetUserConversationsAsync(userId);
    }

    public async Task<Message> AddMessageAsync(int conversationId, string content, MessageSender sender)
    {
        _logger.LogInformation("Adding message to conversation {ConversationId}. Sender: {Sender}", conversationId, sender);

        // Get conversation
        var conversation = await _conversationRepository.GetByIdAsync(conversationId);
        if (conversation == null)
            throw new InvalidOperationException("Conversation not found");

        // Analyze message
        var analysis = await _messageAnalytics.AnalyzeMessageAsync(content);

        // Create message
        var message = new Message
        {
            ConversationId = conversationId,
            Content = analysis.CleanContent,
            Sender = sender,
            SentAt = DateTime.UtcNow,
            Sentiment = analysis.Sentiment,
            SentimentScore = analysis.SentimentScore,
            DetectedIntent = analysis.Intent,
            IntentConfidence = analysis.IntentConfidence,
            IsFiltered = analysis.IsFiltered,
            FilterReason = analysis.FilterReason,
            Conversation = conversation
        };

        await _messageRepository.AddAsync(message);

        // Update conversation
        conversation.LastMessageAt = DateTime.UtcNow;
        conversation.Summary = $"Last message: {analysis.CleanContent} ({analysis.Sentiment})";
        await _conversationRepository.UpdateAsync(conversation);

        return message;
    }

    public async Task<IEnumerable<Message>> GetConversationHistoryAsync(int conversationId)
    {
        return await _messageRepository.GetConversationMessagesAsync(conversationId);
    }

    public async Task<bool> UpdateConversationAsync(int conversationId, string? title = null)
    {
        var conversation = await _conversationRepository.GetByIdAsync(conversationId);
        if (conversation == null)
            return false;

        if (!string.IsNullOrEmpty(title))
            conversation.Title = title;

        await _conversationRepository.UpdateAsync(conversation);
        return true;
    }

    public async Task<string> GenerateBotResponseAsync(int conversationId, string userMessage)
    {
        _logger.LogInformation("Generating bot response for conversation {ConversationId}", conversationId);

        // Get conversation with recent messages
        var conversation = await _conversationRepository.GetWithMessagesAsync(conversationId);
        if (conversation == null)
            throw new InvalidOperationException("Conversation not found");

        // Get recent messages for context
        var recentMessages = conversation.Messages
            .OrderByDescending(m => m.SentAt)
            .Take(10)
            .OrderBy(m => m.SentAt)
            .ToList();

        // Analyze user message
        var analysis = await _messageAnalytics.AnalyzeMessageAsync(userMessage);

        // Generate context-aware response
        var response = _responseTemplateService.GenerateContextAwareResponse(
            userMessage, recentMessages, analysis.Intent ?? "unknown", analysis.Sentiment);

        return response;
    }

    public async Task UpdateConversationSummaryAsync(int conversationId)
    {
        var conversation = await _conversationRepository.GetWithMessagesAsync(conversationId);
        if (conversation == null)
            return;

        var messages = conversation.Messages.OrderBy(m => m.SentAt).ToList();

        // Generate summary and title
        var summary = _summarizationService.GenerateSummary(messages);
        var title = _summarizationService.GenerateTitle(messages);

        // Update conversation
        conversation.Summary = summary;
        if (string.IsNullOrWhiteSpace(conversation.Title) || conversation.Title.StartsWith("Conversation"))
        {
            conversation.Title = title;
        }

        await _conversationRepository.UpdateAsync(conversation);
    }
}
