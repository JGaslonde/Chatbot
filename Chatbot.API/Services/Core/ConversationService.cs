using Chatbot.Core.Models.Entities;
using Chatbot.API.Data.Repositories.Interfaces;
using Chatbot.API.Services.Analysis.Interfaces;
using Chatbot.API.Services.Processing.Interfaces;
using Chatbot.API.Services.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Chatbot.API.Services.Core;

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
    private readonly ILlmResponseService _llmResponseService;
    private readonly IConfiguration _configuration;
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
        ILlmResponseService llmResponseService,
        IConfiguration configuration,
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
        _llmResponseService = llmResponseService;
        _configuration = configuration;
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
        var conversation = await GetConversationAsync(conversationId);
        if (conversation == null)
            throw new InvalidOperationException("Conversation not found");

        var analysis = await _messageAnalytics.AnalyzeMessageAsync(content);

        var message = new Message
        {
            ConversationId = conversationId,
            Content = content,
            Sender = sender,
            SentAt = DateTime.UtcNow,
            Sentiment = analysis.Sentiment,
            SentimentScore = analysis.SentimentScore,
            DetectedIntent = analysis.Intent,
            IntentConfidence = analysis.IntentConfidence,
            IsFiltered = analysis.IsFiltered,
            Conversation = conversation
        };

        var savedMessage = await _messageRepository.AddAsync(message);
        conversation.LastMessageAt = DateTime.UtcNow;
        await _conversationRepository.UpdateAsync(conversation);

        return savedMessage;
    }

    public async Task<IEnumerable<Message>> GetConversationHistoryAsync(int conversationId)
    {
        return await _messageRepository.GetConversationMessagesAsync(conversationId);
    }

    public async Task<bool> UpdateConversationAsync(int conversationId, string? title = null)
    {
        var conversation = await GetConversationAsync(conversationId);
        if (conversation == null)
            return false;

        if (!string.IsNullOrEmpty(title))
            conversation.Title = title;

        await _conversationRepository.UpdateAsync(conversation);
        return true;
    }

    public async Task<string> GenerateBotResponseAsync(int conversationId, string userMessage)
    {
<<<<<<< HEAD
        var conversation = await GetConversationAsync(conversationId);
        if (conversation == null)
            throw new InvalidOperationException("Conversation not found");

        var analysis = await _messageAnalytics.AnalyzeMessageAsync(userMessage);
        var recentMessages = (await GetConversationHistoryAsync(conversationId)).ToList();

        var response = _responseTemplateService.GenerateContextAwareResponse(
            userMessage,
            recentMessages,
            analysis.Intent ?? "unknown",
            analysis.Sentiment
        );

        return response;
=======
        _logger.LogInformation("Generating bot response for conversation {ConversationId}", conversationId);

        var conversation = await _conversationRepository.GetWithMessagesAsync(conversationId);
        if (conversation == null)
            throw new InvalidOperationException("Conversation not found");

        var recentMessages = conversation.Messages
            .OrderByDescending(m => m.SentAt)
            .Take(20)
            .OrderBy(m => m.SentAt)
            .ToList();

        // Try LLM first if enabled
        var llmEnabled = _configuration.GetValue<bool>("Llm:Enabled", false);
        if (llmEnabled)
        {
            var llmResponse = await _llmResponseService.GenerateResponseAsync(userMessage, recentMessages);
            if (!string.IsNullOrWhiteSpace(llmResponse))
                return llmResponse;
        }

        // Fall back to rule-based template responses
        var analysis = await _messageAnalytics.AnalyzeMessageAsync(userMessage);
        return _responseTemplateService.GenerateContextAwareResponse(
            userMessage, recentMessages, analysis.Intent ?? "unknown", analysis.Sentiment);
>>>>>>> fcfb8c252c839cf3d05dc028780147fd1ffddce7
    }

    public async Task UpdateConversationSummaryAsync(int conversationId)
    {
        var conversation = await GetConversationAsync(conversationId);
        if (conversation == null)
            return;

        var messages = (await GetConversationHistoryAsync(conversationId)).ToList();
        conversation.Summary = _summarizationService.GenerateSummary(messages);
        conversation.Title = _summarizationService.GenerateTitle(messages);

        await _conversationRepository.UpdateAsync(conversation);
    }
}
