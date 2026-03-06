using Chatbot.Core.Models.Requests;

namespace Chatbot.API.Services.Phase3.Interfaces;

public interface IAnalyticsExportService
{
    Task<byte[]> ExportAnalyticsReportAsync(int userId, AnalyticsReportRequest request);
    Task<string> GenerateAnalyticsReportJsonAsync(int userId, AnalyticsReportRequest request);
}
