using Chatbot.API.Models.Entities;
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
}

public class ConversationService : IConversationService
{
    private readonly IConversationRepository _conversationRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly ISentimentAnalysisService _sentimentService;
    private readonly IIntentRecognitionService _intentService;
    private readonly IMessageFilterService _filterService;

    public ConversationService(
        IConversationRepository conversationRepository,
        IMessageRepository messageRepository,
        ISentimentAnalysisService sentimentService,
        IIntentRecognitionService intentService,
        IMessageFilterService filterService)
    {
        _conversationRepository = conversationRepository;
        _messageRepository = messageRepository;
        _sentimentService = sentimentService;
        _intentService = intentService;
        _filterService = filterService;
    }

    public async Task<Conversation> CreateConversationAsync(int userId, string? title = null)
    {
        var conversation = new Conversation
        {
            UserId = userId,
            Title = title ?? $"Conversation {DateTime.UtcNow:yyyy-MM-dd HH:mm}",
            StartedAt = DateTime.UtcNow,
            LastMessageAt = DateTime.UtcNow,
            IsActive = true
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
        // Get conversation
        var conversation = await _conversationRepository.GetByIdAsync(conversationId);
        if (conversation == null)
            throw new InvalidOperationException("Conversation not found");

        // Filter message
        var (isClean, issues) = await _filterService.FilterMessageAsync(content);

        // Analyze sentiment and intent
        var (sentiment, sentimentScore) = await _sentimentService.AnalyzeSentimentAsync(content);
        var (intent, intentConfidence) = await _intentService.RecognizeIntentAsync(content);

        // Create message
        var message = new Message
        {
            ConversationId = conversationId,
            Content = isClean ? content : "[Filtered content]",
            Sender = sender,
            SentAt = DateTime.UtcNow,
            Sentiment = sentiment,
            SentimentScore = sentimentScore,
            DetectedIntent = intent,
            IntentConfidence = intentConfidence,
            IsFiltered = !isClean,
            FilterReason = issues.Count > 0 ? string.Join("; ", issues) : null,
            Conversation = conversation
        };

        await _messageRepository.AddAsync(message);

        // Update conversation
        conversation.LastMessageAt = DateTime.UtcNow;
        conversation.Summary = $"Last message: {(isClean ? content : "[Filtered]")} ({sentiment})";
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
}
