using Chatbot.API.Models.Requests;
using Chatbot.API.Models.Responses;

namespace Chatbot.API.Services.Core.Interfaces;

/// <summary>
/// Advanced data export with multiple formats and options
/// </summary>
public interface IAdvancedDataExportService
{
    /// <summary>
    /// Export conversations to various formats
    /// </summary>
    Task<byte[]> ExportConversationsAsync(
        ExportConversationsRequest request,
        int userId);

    /// <summary>
    /// Export single conversation
    /// </summary>
    Task<byte[]> ExportSingleConversationAsync(
        int conversationId,
        string format,
        bool includeMetadata = true);

    /// <summary>
    /// Export conversations matching filter criteria
    /// </summary>
    Task<byte[]> ExportFilteredConversationsAsync(
        ConversationFilterRequest filter,
        string format,
        int userId);

    /// <summary>
    /// Export user data (GDPR compliance)
    /// </summary>
    Task<byte[]> ExportUserDataAsync(
        int userId,
        string format = "json");

    /// <summary>
    /// Export analytics report
    /// </summary>
    Task<byte[]> ExportAnalyticsAsync(
        AnalyticsReportDto report,
        string format);

    /// <summary>
    /// Export activity logs
    /// </summary>
    Task<byte[]> ExportActivityLogsAsync(
        List<ActivityLogDto> logs,
        string format);

    /// <summary>
    /// Create async export job (for large datasets)
    /// </summary>
    Task<string> StartAsyncExportAsync(
        ExportConversationsRequest request,
        int userId);

    /// <summary>
    /// Get status of async export job
    /// </summary>
    Task<Dictionary<string, object>> GetExportJobStatusAsync(string jobId);

    /// <summary>
    /// Download completed export job
    /// </summary>
    Task<byte[]> DownloadExportAsync(string jobId);

    /// <summary>
    /// Cancel export job
    /// </summary>
    Task<bool> CancelExportAsync(string jobId);

    /// <summary>
    /// Get list of recent export jobs
    /// </summary>
    Task<List<Dictionary<string, object>>> GetExportJobHistoryAsync(
        int userId,
        int pageNumber = 1,
        int pageSize = 20);

    /// <summary>
    /// Import conversations from file
    /// </summary>
    Task<BulkOperationResultDto> ImportConversationsAsync(
        byte[] fileContent,
        string format,
        int userId);

    /// <summary>
    /// Get supported export formats
    /// </summary>
    Task<List<string>> GetSupportedFormatsAsync();

    /// <summary>
    /// Get export template (schema)
    /// </summary>
    Task<Dictionary<string, object>> GetExportTemplateAsync(string format);
}
