namespace Chatbot.Core.Models.Requests;

/// <summary>Request to execute a workflow immediately</summary>
public record WorkflowExecutionRequest(
    int WorkflowDefinitionId,
    int ConversationId,
    Dictionary<string, object>? Overrides = null
);
