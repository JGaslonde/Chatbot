namespace Chatbot.Core.Models.Requests;

/// <summary>
/// Request to export conversation data in various formats.
/// </summary>
public record ExportRequest(
    List<int> ConversationIds,
    string Format, // "json", "csv", "pdf"
    bool IncludeMessages = true,
    bool IncludeAnalytics = false
);
