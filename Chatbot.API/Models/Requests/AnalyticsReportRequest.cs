namespace Chatbot.API.Models.Requests;

/// <summary>
/// Request for generating custom analytics reports
/// </summary>
public class AnalyticsReportRequest
{
    /// <summary>
    /// Report title
    /// </summary>
    public string Title { get; set; } = "Analytics Report";

    /// <summary>
    /// Start date for the report period (UTC)
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// End date for the report period (UTC)
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// Report type (summary, detailed, comparative)
    /// </summary>
    public string ReportType { get; set; } = "summary";

    /// <summary>
    /// Metrics to include in report (comma-separated)
    /// </summary>
    public string? Metrics { get; set; }

    /// <summary>
    /// Group results by (day, week, month, none)
    /// </summary>
    public string GroupBy { get; set; } = "day";

    /// <summary>
    /// Include system metrics in report
    /// </summary>
    public bool IncludeSystemMetrics { get; set; } = true;

    /// <summary>
    /// Export format (json, csv, pdf)
    /// </summary>
    public string ExportFormat { get; set; } = "json";
}
