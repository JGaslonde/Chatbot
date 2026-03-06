namespace Chatbot.Core.Models.Responses;

/// <summary>Individual workflow step data</summary>
public record WorkflowStepDto(
    int StepNumber,
    string Action,
    Dictionary<string, object>? Parameters,
    string? Condition
);
