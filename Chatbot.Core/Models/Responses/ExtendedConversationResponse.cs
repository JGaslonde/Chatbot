namespace Chatbot.Core.Models.Responses;

/// <summary>
/// Extended conversation response with additional metadata.
/// </summary>
public record ExtendedConversationResponse(
    int Id,
    string Title,
    DateTime StartedAt,
    int MessageCount,
    string? Summary,
    DateTime? LastMessageAt,
    double? AverageSentiment,
    int DominantIntent,
    bool IsActive
);
