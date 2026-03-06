namespace Chatbot.Core.Models.Responses;

/// <summary>Response with workflow definition</summary>
public record WorkflowDefinitionDto(
    int Id,
    string Name,
    string? Description,
    string TriggerCondition,
    List<WorkflowStepDto> WorkflowSteps,
    bool IsActive,
    int ExecutionCount,
    DateTime CreatedAt,
    DateTime? LastExecutedAt
);
