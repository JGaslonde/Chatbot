using Chatbot.Core.Models.Requests;

namespace Chatbot.API.Services.Export.Interfaces;

/// <summary>
/// Service for exporting conversation data in multiple formats.
/// </summary>
public interface IExportService
{
    /// <summary>
    /// Exports conversations to JSON format.
    /// </summary>
    Task<byte[]> ExportAsJsonAsync(int userId, List<int> conversationIds, bool includeMessages = true);

    /// <summary>
    /// Exports conversations to CSV format.
    /// </summary>
    Task<byte[]> ExportAsCsvAsync(int userId, List<int> conversationIds, bool includeMessages = true);

    /// <summary>
    /// Exports conversations to PDF format.
    /// </summary>
    Task<byte[]> ExportAsPdfAsync(int userId, List<int> conversationIds, bool includeMessages = true);

    /// <summary>
    /// Exports based on the specified format.
    /// </summary>
    Task<byte[]> ExportAsync(int userId, ExportRequest request);
}
