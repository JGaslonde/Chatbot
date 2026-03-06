using Chatbot.API.Data.Repositories.Interfaces;
using Chatbot.Core.Models.Entities;

namespace Chatbot.API.Data.Repositories;

public class WorkflowRepository : Repository<WorkflowDefinition>, IWorkflowRepository
{
    private readonly IRepository<WorkflowExecution> _executionRepository;

    public WorkflowRepository(ChatbotDbContext context, IRepository<WorkflowExecution> executionRepository) : base(context)
    {
        _executionRepository = executionRepository;
    }

    public async Task<List<WorkflowDefinition>> GetUserWorkflowsAsync(int userId) =>
        _context.Set<WorkflowDefinition>().Where(x => x.UserId == userId)
              .OrderByDescending(x => x.CreatedAt)
              .ToList();

    public async Task<List<WorkflowDefinition>> GetActiveWorkflowsAsync(int userId) =>
        _context.Set<WorkflowDefinition>().Where(x => x.UserId == userId && x.IsActive)
              .ToList();

    public async Task<List<WorkflowExecution>> GetWorkflowExecutionsAsync(int workflowId, int limit = 50)
    {
        var workflow = _context.Set<WorkflowDefinition>().FirstOrDefault(x => x.Id == workflowId);
        if (workflow == null) return new List<WorkflowExecution>();
        return _context.Set<WorkflowExecution>().Where(x => x.WorkflowDefinitionId == workflowId)
                      .OrderByDescending(x => x.StartedAt)
                      .Take(limit)
                      .ToList();
    }

    public async Task<WorkflowExecution> LogExecutionAsync(WorkflowExecution execution)
    {
        await _executionRepository.AddAsync(execution);
        return execution;
    }
}
