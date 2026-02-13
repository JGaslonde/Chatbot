namespace Chatbot.Core.Models.Requests;

/// <summary>
/// Request for batch operations on conversations or messages.
/// </summary>
public record BatchOperationRequest(
    string Operation,
    List<int> ConversationIds,
    Dictionary<string, object>? Parameters = null
);
