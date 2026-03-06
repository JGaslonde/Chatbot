namespace Chatbot.Core.Models.Requests;

/// <summary>Request to create or analyze conversation analytics</summary>
public record AnalyticsRequest(
    int ConversationId,
    string? DateRangeStart = null,
    string? DateRangeEnd = null,
    bool IncludeTrendAnalysis = false
);
