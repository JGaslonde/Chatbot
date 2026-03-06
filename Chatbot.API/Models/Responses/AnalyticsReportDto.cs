namespace Chatbot.API.Models.Responses;

/// <summary>
/// Analytics report data response
/// </summary>
public class AnalyticsReportDto
{
    /// <summary>
    /// Report ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Report title
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Report period start
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Report period end
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// When report was generated
    /// </summary>
    public DateTime GeneratedAt { get; set; }

    /// <summary>
    /// Total conversations analyzed
    /// </summary>
    public int TotalConversations { get; set; }

    /// <summary>
    /// Total messages processed
    /// </summary>
    public long TotalMessages { get; set; }

    /// <summary>
    /// Average sentiment score
    /// </summary>
    public double AverageSentiment { get; set; }

    /// <summary>
    /// Time series metrics data
    /// </summary>
    public List<MetricDataPoint> MetricsTimeSeries { get; set; } = new();

    /// <summary>
    /// Summary statistics
    /// </summary>
    public Dictionary<string, object> SummaryStats { get; set; } = new();

    /// <summary>
    /// Report format
    /// </summary>
    public string Format { get; set; } = "json";
}

/// <summary>
/// Single data point in time series analytics
/// </summary>
public class MetricDataPoint
{
    /// <summary>
    /// Timestamp
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Conversation count
    /// </summary>
    public int ConversationCount { get; set; }

    /// <summary>
    /// Message count
    /// </summary>
    public int MessageCount { get; set; }

    /// <summary>
    /// Average sentiment for period
    /// </summary>
    public double AverageSentiment { get; set; }

    /// <summary>
    /// Custom metrics
    /// </summary>
    public Dictionary<string, object> CustomMetrics { get; set; } = new();
}

/// <summary>
/// System metrics and health data
/// </summary>
public class SystemMetricsDto
{
    /// <summary>
    /// API uptime in hours
    /// </summary>
    public double UptimeHours { get; set; }

    /// <summary>
    /// Request count in period
    /// </summary>
    public long RequestCount { get; set; }

    /// <summary>
    /// Average response time in ms
    /// </summary>
    public double AverageResponseTimeMs { get; set; }

    /// <summary>
    /// Error rate percentage
    /// </summary>
    public double ErrorRatePercent { get; set; }

    /// <summary>
    /// Database connection pool status
    /// </summary>
    public string DatabaseStatus { get; set; } = "healthy";

    /// <summary>
    /// Cache hit rate
    /// </summary>
    public double CacheHitRatePercent { get; set; }

    /// <summary>
    /// Active user sessions
    /// </summary>
    public int ActiveSessions { get; set; }

    /// <summary>
    /// Memory usage percentage
    /// </summary>
    public double MemoryUsagePercent { get; set; }

    /// <summary>
    /// CPU usage percentage
    /// </summary>
    public double CpuUsagePercent { get; set; }

    /// <summary>
    /// Queue depth
    /// </summary>
    public int QueueDepth { get; set; }

    /// <summary>
    /// Last health check time
    /// </summary>
    public DateTime LastHealthCheck { get; set; }
}

/// <summary>
/// Bulk operation result
/// </summary>
public class BulkOperationResultDto
{
    /// <summary>
    /// Operation ID
    /// </summary>
    public string OperationId { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Operation type that was performed
    /// </summary>
    public string OperationType { get; set; } = string.Empty;

    /// <summary>
    /// Number of items processed
    /// </summary>
    public int ProcessedCount { get; set; }

    /// <summary>
    /// Number of successful operations
    /// </summary>
    public int SuccessCount { get; set; }

    /// <summary>
    /// Number of failed operations
    /// </summary>
    public int FailureCount { get; set; }

    /// <summary>
    /// Operation start time
    /// </summary>
    public DateTime StartedAt { get; set; }

    /// <summary>
    /// Operation end time
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// Errors encountered
    /// </summary>
    public List<string> Errors { get; set; } = new();

    /// <summary>
    /// Warning messages
    /// </summary>
    public List<string> Warnings { get; set; } = new();

    /// <summary>
    /// Operation status
    /// </summary>
    public string Status { get; set; } = "completed";
}
