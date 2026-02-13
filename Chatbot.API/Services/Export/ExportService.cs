using System.Text;
using System.Text.Json;
using Chatbot.Core.Models.Requests;
using Chatbot.API.Data.Repositories.Interfaces;
using Chatbot.API.Services.Export.Interfaces;
using Microsoft.Extensions.Logging;

namespace Chatbot.API.Services.Export;

public class ExportService : IExportService
{
    private readonly IConversationRepository _conversationRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly ILogger<ExportService> _logger;

    public ExportService(
        IConversationRepository conversationRepository,
        IMessageRepository messageRepository,
        ILogger<ExportService> logger)
    {
        _conversationRepository = conversationRepository;
        _messageRepository = messageRepository;
        _logger = logger;
    }

    public async Task<byte[]> ExportAsJsonAsync(int userId, List<int> conversationIds, bool includeMessages = true)
    {
        try
        {
            var conversations = new List<dynamic>();

            foreach (var convId in conversationIds)
            {
                var conversation = await _conversationRepository.GetWithMessagesAsync(convId);
                if (conversation?.UserId == userId)
                {
                    var data = new
                    {
                        conversation.Id,
                        conversation.Title,
                        conversation.StartedAt,
                        conversation.Summary,
                        Messages = includeMessages ? conversation.Messages?.Select(m => new
                        {
                            m.Id,
                            m.Content,
                            m.Sender,
                            m.SentAt,
                            m.Sentiment,
                            m.SentimentScore
                        }).ToList() : null
                    };
                    conversations.Add(data);
                }
            }

            var json = JsonSerializer.Serialize(conversations, new JsonSerializerOptions { WriteIndented = true });
            return Encoding.UTF8.GetBytes(json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting conversations to JSON for user {UserId}", userId);
            throw;
        }
    }

    public async Task<byte[]> ExportAsCsvAsync(int userId, List<int> conversationIds, bool includeMessages = true)
    {
        try
        {
            var sb = new StringBuilder();
            sb.AppendLine("ConversationId,Title,StartedAt,Summary");

            foreach (var convId in conversationIds)
            {
                var conversation = await _conversationRepository.GetWithMessagesAsync(convId);
                if (conversation?.UserId == userId)
                {
                    // Escape CSV fields
                    var title = conversation.Title?.Replace("\"", "\"\"") ?? "";
                    var summary = conversation.Summary?.Replace("\"", "\"\"") ?? "";
                    
                    sb.AppendLine($"{conversation.Id},\"{title}\",{conversation.StartedAt:O},\"{summary}\"");

                    if (includeMessages)
                    {
                        foreach (var msg in conversation.Messages ?? new List<Chatbot.Core.Models.Entities.Message>())
                        {
                            var content = msg.Content.Replace("\"", "\"\"");
                            sb.AppendLine($"{msg.Id},\"{content}\",{msg.SentAt:O},{msg.Sentiment}");
                        }
                    }
                }
            }

            return Encoding.UTF8.GetBytes(sb.ToString());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting conversations to CSV for user {UserId}", userId);
            throw;
        }
    }

    public async Task<byte[]> ExportAsPdfAsync(int userId, List<int> conversationIds, bool includeMessages = true)
    {
        // PDF export requires external library like iTextSharp
        // For now, return a placeholder implementation
        // In production, integrate a PDF library
        try
        {
            var textContent = new StringBuilder();
            textContent.AppendLine("Conversation Export Report");
            textContent.AppendLine(DateTime.UtcNow.ToString("O"));
            textContent.AppendLine("");

            foreach (var convId in conversationIds)
            {
                var conversation = await _conversationRepository.GetWithMessagesAsync(convId);
                if (conversation?.UserId == userId)
                {
                    textContent.AppendLine($"Conversation: {conversation.Title} (ID: {conversation.Id})");
                    textContent.AppendLine($"Started: {conversation.StartedAt}");
                    textContent.AppendLine($"Summary: {conversation.Summary}");

                    if (includeMessages)
                    {
                        foreach (var msg in conversation.Messages ?? new List<Chatbot.Core.Models.Entities.Message>())
                        {
                            textContent.AppendLine($"  [{msg.Sender}] {msg.SentAt}: {msg.Content}");
                        }
                    }
                    textContent.AppendLine("");
                }
            }

            // For now, return as plain text encoded as bytes
            // Real PDF implementation would use iTextSharp or similar
            return Encoding.UTF8.GetBytes(textContent.ToString());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting conversations to PDF for user {UserId}", userId);
            throw;
        }
    }

    public async Task<byte[]> ExportAsync(int userId, ExportRequest request)
    {
        return request.Format.ToLower() switch
        {
            "json" => await ExportAsJsonAsync(userId, request.ConversationIds, request.IncludeMessages),
            "csv" => await ExportAsCsvAsync(userId, request.ConversationIds, request.IncludeMessages),
            "pdf" => await ExportAsPdfAsync(userId, request.ConversationIds, request.IncludeMessages),
            _ => throw new ArgumentException($"Unsupported export format: {request.Format}")
        };
    }
}
