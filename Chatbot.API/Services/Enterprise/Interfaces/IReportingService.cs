using Chatbot.Core.Models.Requests;
using Chatbot.Core.Models.Responses;

namespace Chatbot.API.Services.Core.Interfaces;

/// <summary>
/// Service for advanced reporting.
/// </summary>
public interface IReportingService
{
    Task<ScheduledReportDto> CreateScheduledReportAsync(int userId, ScheduledReportRequest request);
    Task<List<ScheduledReportDto>> GetUserReportsAsync(int userId);
    Task<byte[]> GenerateReportAsync(int userId, int reportId, string format = "pdf");
    Task<bool> UpdateScheduledReportAsync(int userId, int reportId, ScheduledReportRequest request);
    Task<bool> DeleteScheduledReportAsync(int userId, int reportId);
    Task ProcessScheduledReportsAsync();
}
