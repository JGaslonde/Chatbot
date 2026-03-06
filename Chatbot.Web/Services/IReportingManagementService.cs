using Chatbot.Core.Models.Requests;
using Chatbot.Core.Models.Responses;

namespace Chatbot.Web.Services;

public interface IReportingManagementService
{
    Task<ScheduledReportDto?> CreateScheduledReportAsync(ScheduledReportRequest request);
    Task<List<ScheduledReportDto>> GetReportsAsync();
    Task<bool> UpdateScheduledReportAsync(int reportId, ScheduledReportRequest request);
    Task<bool> DeleteScheduledReportAsync(int reportId);
    Task<byte[]?> GenerateReportAsync(int reportId, string format = "pdf");
}
