using System.Text;
using System.Text.Json;
using Chatbot.API.Data.Repositories;
using Chatbot.Core.Models.Entities;

namespace Chatbot.API.Services.Export;

public interface IConversationExportService
{
    Task<string> ExportToJsonAsync(int conversationId);
    Task<string> ExportToCsvAsync(int conversationId);
    Task<byte[]> ExportToJsonBytesAsync(int conversationId);
    Task<byte[]> ExportToCsvBytesAsync(int conversationId);
}

public class ConversationExportService : IConversationExportService
{
    private readonly IConversationRepository _conversationRepository;
    private readonly IMessageRepository _messageRepository;

    public ConversationExportService(
        IConversationRepository conversationRepository,
        IMessageRepository messageRepository)
    {
        _conversationRepository = conversationRepository;
        _messageRepository = messageRepository;
    }

    public async Task<string> ExportToJsonAsync(int conversationId)
    {
        var conversation = await _conversationRepository.GetByIdAsync(conversationId);
        if (conversation == null)
            throw new InvalidOperationException($"Conversation {conversationId} not found");

        var messages = (await _messageRepository.GetAllAsync())
            .Where(m => m.ConversationId == conversationId)
            .OrderBy(m => m.SentAt)
            .Select(m => new
            {
                m.Id,
                m.Content,
                Sender = m.Sender.ToString(),
                SentAt = m.SentAt.ToString("o"),
                Sentiment = m.Sentiment.ToString(),
                m.SentimentScore,
                m.DetectedIntent,
                m.IntentConfidence
            })
            .ToList();

        var export = new
        {
            ConversationId = conversation.Id,
            conversation.Title,
            conversation.Summary,
            StartedAt = conversation.StartedAt.ToString("o"),
            LastMessageAt = conversation.LastMessageAt.ToString("o"),
            MessageCount = messages.Count,
            Messages = messages,
            ExportedAt = DateTime.UtcNow.ToString("o")
        };

        return JsonSerializer.Serialize(export, new JsonSerializerOptions
        {
            WriteIndented = true
        });
    }

    public async Task<string> ExportToCsvAsync(int conversationId)
    {
        var conversation = await _conversationRepository.GetByIdAsync(conversationId);
        if (conversation == null)
            throw new InvalidOperationException($"Conversation {conversationId} not found");

        var messages = (await _messageRepository.GetAllAsync())
            .Where(m => m.ConversationId == conversationId)
            .OrderBy(m => m.SentAt)
            .ToList();

        var csv = new StringBuilder();

        // Header
        csv.AppendLine("MessageId,Sender,Content,SentAt,Sentiment,SentimentScore,DetectedIntent,IntentConfidence");

        // Data rows
        foreach (var message in messages)
        {
            var content = EscapeCsvField(message.Content);
            var sentAt = message.SentAt.ToString("o");
            var sentiment = message.Sentiment.ToString();
            var intent = message.DetectedIntent ?? "";

            csv.AppendLine($"{message.Id},{message.Sender},{content},{sentAt},{sentiment},{message.SentimentScore},{intent},{message.IntentConfidence}");
        }

        return csv.ToString();
    }

    public async Task<byte[]> ExportToJsonBytesAsync(int conversationId)
    {
        var json = await ExportToJsonAsync(conversationId);
        return Encoding.UTF8.GetBytes(json);
    }

    public async Task<byte[]> ExportToCsvBytesAsync(int conversationId)
    {
        var csv = await ExportToCsvAsync(conversationId);
        return Encoding.UTF8.GetBytes(csv);
    }

    private static string EscapeCsvField(string field)
    {
        if (field.Contains(',') || field.Contains('"') || field.Contains('\n'))
        {
            return $"\"{field.Replace("\"", "\"\"")}\"";
        }
        return field;
    }
}
