using Chatbot.API.Data.Repositories.Interfaces;
using Chatbot.API.Services.Core.Interfaces;
using Chatbot.Core.Models.Entities;
using Chatbot.Core.Models.Requests;
using Chatbot.Core.Models.Responses;

namespace Chatbot.API.Services.Core;

public class ReportingService : IReportingService
{
    private readonly IScheduledReportRepository _repository;
    private readonly ILogger<ReportingService> _logger;

    public ReportingService(IScheduledReportRepository repository, ILogger<ReportingService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<ScheduledReportDto> CreateScheduledReportAsync(int userId, ScheduledReportRequest request)
    {
        try
        {
            var report = new ScheduledReport
            {
                UserId = userId,
                Name = request.Name,
                Description = request.Description,
                ReportType = request.ReportType,
                Frequency = request.Frequency,
                RecipientEmail = request.RecipientEmail,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                NextGeneratedAt = CalculateNextGenerationTime(request.Frequency)
            };

            await _repository.AddAsync(report);

            _logger.LogInformation($"Scheduled report created for user {userId}: {report.Id}");

            return MapToDto(report);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error creating scheduled report: {ex.Message}");
            throw;
        }
    }

    public async Task<List<ScheduledReportDto>> GetUserReportsAsync(int userId)
    {
        var reports = await _repository.GetUserReportsAsync(userId);
        return reports.Select(MapToDto).ToList();
    }

    public async Task<byte[]> GenerateReportAsync(int userId, int reportId, string format = "pdf")
    {
        var report = await _repository.GetByIdAsync(reportId);
        if (report == null || report.UserId != userId)
            throw new UnauthorizedAccessException("Report not found");

        // Generate report based on format
        var content = format.ToLower() switch
        {
            "json" => GenerateJsonReport(report),
            "csv" => GenerateCsvReport(report),
            "pdf" => GeneratePdfReport(report),
            _ => throw new ArgumentException("Invalid format")
        };

        return content;
    }

    public async Task<bool> UpdateScheduledReportAsync(int userId, int reportId, ScheduledReportRequest request)
    {
        var report = await _repository.GetByIdAsync(reportId);
        if (report == null || report.UserId != userId)
            return false;

        report.Name = request.Name;
        report.Description = request.Description;
        report.ReportType = request.ReportType;
        report.Frequency = request.Frequency;
        report.RecipientEmail = request.RecipientEmail;

        await _repository.UpdateAsync(report);

        return true;
    }

    public async Task<bool> DeleteScheduledReportAsync(int userId, int reportId)
    {
        var report = await _repository.GetByIdAsync(reportId);
        if (report == null || report.UserId != userId)
            return false;

        await _repository.DeleteAsync(reportId);

        _logger.LogInformation($"Scheduled report deleted for user {userId}: {reportId}");
        return true;
    }

    public async Task ProcessScheduledReportsAsync()
    {
        var dueReports = await _repository.GetDueReportsAsync();

        foreach (var report in dueReports)
        {
            try
            {
                // Generate report
                var reportContent = GenerateJsonReport(report);

                // Update report timestamps
                report.LastGeneratedAt = DateTime.UtcNow;
                report.NextGeneratedAt = CalculateNextGenerationTime(report.Frequency);

                await _repository.UpdateAsync(report);

                _logger.LogInformation($"Report {report.Id} generated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing scheduled report {report.Id}: {ex.Message}");
            }
        }
    }

    private static byte[] GenerateJsonReport(ScheduledReport report)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(new
        {
            report.Name,
            report.ReportType,
            report.Frequency,
            GeneratedAt = DateTime.UtcNow,
            Data = new { } // Placeholder for actual report data
        });

        return System.Text.Encoding.UTF8.GetBytes(json);
    }

    private static byte[] GenerateCsvReport(ScheduledReport report)
    {
        var csv = $"Name,Type,Frequency,GeneratedAt\n{report.Name},{report.ReportType},{report.Frequency},{DateTime.UtcNow}";
        return System.Text.Encoding.UTF8.GetBytes(csv);
    }

    private static byte[] GeneratePdfReport(ScheduledReport report)
    {
        // Simplified PDF generation - in production use iTextSharp or similar
        var text = $"Report: {report.Name}\nType: {report.ReportType}\nGenerated: {DateTime.UtcNow}";
        return System.Text.Encoding.UTF8.GetBytes(text);
    }

    private static DateTime CalculateNextGenerationTime(string frequency)
    {
        return frequency.ToLower() switch
        {
            "daily" => DateTime.UtcNow.AddDays(1),
            "weekly" => DateTime.UtcNow.AddDays(7),
            "monthly" => DateTime.UtcNow.AddMonths(1),
            _ => DateTime.UtcNow.AddDays(1)
        };
    }

    private static ScheduledReportDto MapToDto(ScheduledReport report)
    {
        return new ScheduledReportDto(
            report.Id,
            report.Name,
            report.Description,
            report.ReportType,
            report.Frequency,
            report.RecipientEmail,
            report.IsActive,
            report.LastGeneratedAt,
            report.NextGeneratedAt
        );
    }
}
