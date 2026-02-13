using Chatbot.Core.Models.Requests;
using Chatbot.Core.Models.Responses;

namespace Chatbot.API.Services.Analytics.Interfaces;

/// <summary>
/// Service for advanced analytics on conversations and messages.
/// </summary>
public interface IAdvancedAnalyticsService
{
    /// <summary>
    /// Gets comprehensive analytics for a date range.
    /// </summary>
    Task<AdvancedAnalyticsResponse> GetAdvancedAnalyticsAsync(
        int userId,
        DateTime fromDate,
        DateTime toDate);

    /// <summary>
    /// Gets trend data over time periods.
    /// </summary>
    Task<Dictionary<string, int>> GetMessageTrendsByDayAsync(
        int userId,
        DateTime fromDate,
        DateTime toDate);

    /// <summary>
    /// Gets intent distribution across all conversations.
    /// </summary>
    Task<Dictionary<string, int>> GetIntentDistributionAsync(int userId);
}
