namespace Chatbot.API.Services.Export;

public interface IConversationExportService
{
    Task<string> ExportToJsonAsync(int conversationId);
    Task<string> ExportToCsvAsync(int conversationId);
    Task<byte[]> ExportToJsonBytesAsync(int conversationId);
    Task<byte[]> ExportToCsvBytesAsync(int conversationId);
}
