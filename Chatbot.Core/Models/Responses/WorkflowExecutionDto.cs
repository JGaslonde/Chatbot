namespace Chatbot.Core.Models.Responses;

/// <summary>Response with workflow execution history</summary>
public record WorkflowExecutionDto(
    int Id,
    int WorkflowDefinitionId,
    int ConversationId,
    string Status,
    string? ErrorMessage,
    DateTime StartedAt,
    DateTime? CompletedAt
);
