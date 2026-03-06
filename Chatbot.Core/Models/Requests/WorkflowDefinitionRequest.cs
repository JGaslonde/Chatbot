namespace Chatbot.Core.Models.Requests;

/// <summary>Request to create a workflow automation definition</summary>
public record WorkflowDefinitionRequest(
    string Name,
    string? Description,
    string TriggerCondition,
    List<WorkflowStepRequest> Steps,
    bool IsActive = true
);
