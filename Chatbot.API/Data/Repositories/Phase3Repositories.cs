using Chatbot.API.Data.Repositories.Interfaces;
using Chatbot.Core.Models.Entities;
using Chatbot.API.Data.Context;

namespace Chatbot.API.Data.Repositories;

public interface IPhase3Repositories
{
    IConversationAnalyticsRepository ConversationAnalytics { get; }
    IMLInsightRepository MLInsight { get; }
    IWorkflowRepository Workflow { get; }
    IUserSegmentRepository UserSegment { get; }
    ISearchIndexRepository SearchIndex { get; }
}

public interface IConversationAnalyticsRepository : IRepository<ConversationAnalyticsEntity>
{
    Task<ConversationAnalyticsEntity?> GetByConversationIdAsync(int conversationId);
    Task<List<ConversationAnalyticsEntity>> GetUserAnalyticsByDateRangeAsync(int userId, DateTime startDate, DateTime endDate);
    Task<double> GetAverageSentimentAsync(int userId, int days = 30);
    Task<List<ConversationAnalyticsEntity>> GetByEngagementScoreAsync(int userId, int minScore);
}

public interface IMLInsightRepository : IRepository<MLInsight>
{
    Task<List<MLInsight>> GetUserInsightsAsync(int userId);
    Task<List<MLInsight>> GetByInsightTypeAsync(int userId, string insightType);
    Task<List<MLInsight>> GetUnreviewedInsightsAsync(int userId);
    Task<List<MLInsight>> GetHighConfidenceInsightsAsync(int userId, double minConfidence = 0.8);
}

public interface IWorkflowRepository : IRepository<WorkflowDefinition>
{
    Task<List<WorkflowDefinition>> GetUserWorkflowsAsync(int userId);
    Task<List<WorkflowDefinition>> GetActiveWorkflowsAsync(int userId);
    Task<List<WorkflowExecution>> GetWorkflowExecutionsAsync(int workflowId, int limit = 50);
    Task<WorkflowExecution> LogExecutionAsync(WorkflowExecution execution);
}

public interface IUserSegmentRepository : IRepository<UserSegment>
{
    Task<UserSegment?> GetByUserIdAsync(int userId);
    Task<List<UserSegment>> GetByBehavioralSegmentAsync(string segment);
    Task<List<UserSegment>> GetChurnRiskUsersAsync(double minRiskScore = 0.7);
    Task<List<UserSegment>> GetEngagementLevelAsync(string engagementLevel);
}

public interface ISearchIndexRepository : IRepository<SearchIndex>
{
    Task<List<SearchIndex>> SearchContentAsync(int userId, string query);
    Task<List<SearchIndex>> SearchByTopicAsync(int userId, string topic);
    Task<List<SearchIndex>> SearchByIntentAsync(int userId, string intent);
    Task<SearchIndex?> GetByConversationIdAsync(int conversationId);
    Task RebuildIndexAsync(int userId);
}

// Repository Implementations
public class ConversationAnalyticsRepository : Repository<ConversationAnalyticsEntity>, IConversationAnalyticsRepository
{
    public ConversationAnalyticsRepository(ChatbotDbContext context) : base(context) { }

    public async Task<ConversationAnalyticsEntity?> GetByConversationIdAsync(int conversationId) =>
        _context.Set<ConversationAnalyticsEntity>().FirstOrDefault(x => x.ConversationId == conversationId);

    public async Task<List<ConversationAnalyticsEntity>> GetUserAnalyticsByDateRangeAsync(int userId, DateTime startDate, DateTime endDate) =>
        _context.Set<ConversationAnalyticsEntity>()
              .Where(x => x.UserId == userId && x.UpdatedAt >= startDate && x.UpdatedAt <= endDate)
              .OrderByDescending(x => x.UpdatedAt)
              .ToList();

    public async Task<double> GetAverageSentimentAsync(int userId, int days = 30)
    {
        var since = DateTime.UtcNow.AddDays(-days);
        var analytics = _context.Set<ConversationAnalyticsEntity>().Where(x => x.UserId == userId && x.CreatedAt >= since).ToList();
        return analytics.Any() ? analytics.Average(x => x.AverageSentimentScore) : 0.0;
    }

    public async Task<List<ConversationAnalyticsEntity>> GetByEngagementScoreAsync(int userId, int minScore) =>
        _context.Set<ConversationAnalyticsEntity>()
              .Where(x => x.UserId == userId && x.EngagementScore >= minScore)
              .OrderByDescending(x => x.EngagementScore)
              .ToList();
}

public class MLInsightRepository : Repository<MLInsight>, IMLInsightRepository
{
    public MLInsightRepository(ChatbotDbContext context) : base(context) { }

    public async Task<List<MLInsight>> GetUserInsightsAsync(int userId) =>
        _context.Set<MLInsight>().Where(x => x.UserId == userId)
              .OrderByDescending(x => x.GeneratedAt)
              .ToList();

    public async Task<List<MLInsight>> GetByInsightTypeAsync(int userId, string insightType) =>
        _context.Set<MLInsight>().Where(x => x.UserId == userId && x.InsightType == insightType)
              .OrderByDescending(x => x.Confidence)
              .ToList();

    public async Task<List<MLInsight>> GetUnreviewedInsightsAsync(int userId) =>
        _context.Set<MLInsight>().Where(x => x.UserId == userId && !x.IsReviewed)
              .OrderByDescending(x => x.GeneratedAt)
              .ToList();

    public async Task<List<MLInsight>> GetHighConfidenceInsightsAsync(int userId, double minConfidence) =>
        _context.Set<MLInsight>().Where(x => x.UserId == userId && x.Confidence >= minConfidence)
              .OrderByDescending(x => x.Confidence)
              .ToList();
}

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

public class UserSegmentRepository : Repository<UserSegment>, IUserSegmentRepository
{
    public UserSegmentRepository(ChatbotDbContext context) : base(context) { }

    public async Task<UserSegment?> GetByUserIdAsync(int userId) =>
        _context.Set<UserSegment>().FirstOrDefault(x => x.UserId == userId);

    public async Task<List<UserSegment>> GetByBehavioralSegmentAsync(string segment) =>
        _context.Set<UserSegment>().Where(x => x.BehavioralSegment == segment)
              .OrderByDescending(x => x.ChurnRiskScore)
              .ToList();

    public async Task<List<UserSegment>> GetChurnRiskUsersAsync(double minRiskScore) =>
        _context.Set<UserSegment>().Where(x => x.ChurnRiskScore >= minRiskScore)
              .OrderByDescending(x => x.ChurnRiskScore)
              .ToList();

    public async Task<List<UserSegment>> GetEngagementLevelAsync(string engagementLevel) =>
        _context.Set<UserSegment>().Where(x => x.EngagementLevel == engagementLevel)
              .ToList();
}

public class SearchIndexRepository : Repository<SearchIndex>, ISearchIndexRepository
{
    public SearchIndexRepository(ChatbotDbContext context) : base(context) { }

    public async Task<List<SearchIndex>> SearchContentAsync(int userId, string query)
    {
        var lowerQuery = query.ToLower();
        return _context.Set<SearchIndex>().Where(x => x.UserId == userId && x.Content.ToLower().Contains(lowerQuery))
                     .OrderByDescending(x => x.RelevanceScore)
                     .ToList();
    }

    public async Task<List<SearchIndex>> SearchByTopicAsync(int userId, string topic) =>
        _context.Set<SearchIndex>().Where(x => x.UserId == userId && x.Topics != null && x.Topics.Contains(topic))
              .OrderByDescending(x => x.IndexedAt)
              .ToList();

    public async Task<List<SearchIndex>> SearchByIntentAsync(int userId, string intent) =>
        _context.Set<SearchIndex>().Where(x => x.UserId == userId && x.Intents != null && x.Intents.Contains(intent))
              .OrderByDescending(x => x.IndexedAt)
              .ToList();

    public async Task<SearchIndex?> GetByConversationIdAsync(int conversationId) =>
        _context.Set<SearchIndex>().FirstOrDefault(x => x.ConversationId == conversationId);

    public async Task RebuildIndexAsync(int userId)
    {
        // In a real implementation, this would rebuild the search index from conversation data
        var existingIndexes = _context.Set<SearchIndex>().Where(x => x.UserId == userId).ToList();
        foreach (var index in existingIndexes)
        {
            index.IndexedAt = DateTime.UtcNow;
        }
    }
}
