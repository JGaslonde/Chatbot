using System.Text;
using System.Text.Json;
using Chatbot.API.Data.Repositories.Interfaces;
using Chatbot.API.Services.Export.Interfaces;

namespace Chatbot.API.Services.Export;

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
        return Encoding.UTF8.GetString(await ExportToJsonBytesAsync(conversationId));
    }

    public async Task<string> ExportToCsvAsync(int conversationId)
    {
        return Encoding.UTF8.GetString(await ExportToCsvBytesAsync(conversationId));
    }

    public async Task<byte[]> ExportToJsonBytesAsync(int conversationId)
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

        var json = JsonSerializer.Serialize(export, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        return Encoding.UTF8.GetBytes(json);
    }

    public async Task<byte[]> ExportToCsvBytesAsync(int conversationId)
    {
        var conversation = await _conversationRepository.GetByIdAsync(conversationId);
        if (conversation == null)
            throw new InvalidOperationException($"Conversation {conversationId} not found");

        var messages = (await _messageRepository.GetAllAsync())
            .Where(m => m.ConversationId == conversationId)
            .OrderBy(m => m.SentAt)
            .ToList();

        var csv = new StringBuilder();
        csv.AppendLine("Conversation Export");
        csv.AppendLine($"Title,{CsvEscape(conversation.Title)}");
        csv.AppendLine($"Started At,{conversation.StartedAt:yyyy-MM-dd HH:mm:ss}");
        csv.AppendLine($"Last Message,{conversation.LastMessageAt:yyyy-MM-dd HH:mm:ss}");
        csv.AppendLine($"Message Count,{messages.Count}");
        csv.AppendLine();

        csv.AppendLine("Timestamp,Sender,Content,Sentiment,SentimentScore,Intent,IntentConfidence");
        foreach (var message in messages)
        {
            csv.AppendLine($"{message.SentAt:yyyy-MM-dd HH:mm:ss}," +
                          $"{message.Sender}," +
                          $"{CsvEscape(message.Content)}," +
                          $"{message.Sentiment}," +
                          $"{message.SentimentScore:F2}," +
                          $"{message.DetectedIntent ?? "N/A"}," +
                          $"{message.IntentConfidence:F2}");
        }

        return Encoding.UTF8.GetBytes(csv.ToString());
    }

    private string CsvEscape(string value)
    {
        if (string.IsNullOrEmpty(value))
            return "";

        if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
        {
            return $"\"{value.Replace("\"", "\"\"")}\"";
        }

        return value;
    }
}
