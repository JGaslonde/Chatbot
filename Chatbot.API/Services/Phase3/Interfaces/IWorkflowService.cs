using Chatbot.Core.Models.Requests;
using Chatbot.Core.Models.Responses;

namespace Chatbot.API.Services.Phase3.Interfaces;

public interface IWorkflowService
{
    Task<List<WorkflowDefinitionDto>> GetUserWorkflowsAsync(int userId);
    Task<WorkflowDefinitionDto?> GetWorkflowAsync(int workflowId);
    Task<WorkflowDefinitionDto> CreateWorkflowAsync(int userId, WorkflowDefinitionRequest request);
    Task<WorkflowDefinitionDto> UpdateWorkflowAsync(int workflowId, WorkflowDefinitionRequest request);
    Task<WorkflowExecutionDto> ExecuteWorkflowAsync(int workflowId, int conversationId, WorkflowExecutionRequest request);
    Task<List<WorkflowExecutionDto>> GetWorkflowExecutionsAsync(int workflowId, int limit = 50);
}
