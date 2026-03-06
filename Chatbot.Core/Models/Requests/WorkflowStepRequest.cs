namespace Chatbot.Core.Models.Requests;

/// <summary>Individual step in a workflow</summary>
public record WorkflowStepRequest(
    int StepNumber,
    string Action,
    Dictionary<string, object>? Parameters = null,
    string? Condition = null
);
