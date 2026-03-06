using Chatbot.Core.Models.Entities;

namespace Chatbot.API.Data.Repositories.Interfaces;

public interface IWorkflowRepository : IRepository<WorkflowDefinition>
{
    Task<List<WorkflowDefinition>> GetUserWorkflowsAsync(int userId);
    Task<List<WorkflowDefinition>> GetActiveWorkflowsAsync(int userId);
    Task<List<WorkflowExecution>> GetWorkflowExecutionsAsync(int workflowId, int limit = 50);
    Task<WorkflowExecution> LogExecutionAsync(WorkflowExecution execution);
}
