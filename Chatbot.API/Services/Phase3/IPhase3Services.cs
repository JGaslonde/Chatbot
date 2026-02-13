using Chatbot.Core.Models.Entities;
using Chatbot.Core.Models.Requests;
using Chatbot.Core.Models.Responses;

namespace Chatbot.API.Services.Phase3.Interfaces;

public interface IConversationAnalyticsService
{
    Task<ConversationAnalyticsDto?> GetAnalyticsAsync(int conversationId);
    Task<List<ConversationAnalyticsDto>> GetUserAnalyticsByDateRangeAsync(int userId, DateTime startDate, DateTime endDate);
    Task<double> GetAverageSentimentAsync(int userId, int days = 30);
    Task<AnalyticsSummaryDto> GetAnalyticsSummaryAsync(int userId, int days = 30);
    Task<ConversationAnalyticsEntity> CreateOrUpdateAnalyticsAsync(int conversationId, int userId, AnalyticsRequest request);
}

public interface IMLInsightService
{
    Task<List<MLInsightDto>> GetUserInsightsAsync(int userId);
    Task<List<MLInsightDto>> GetByInsightTypeAsync(int userId, string insightType);
    Task<List<MLInsightDto>> GetUnreviewedInsightsAsync(int userId);
    Task<MLInsightDto> MarkAsReviewedAsync(int insightId);
    Task<MLInsight> GenerateInsightAsync(int userId, int conversationId, string insightType, double confidence, string value);
}

public interface IWorkflowService
{
    Task<List<WorkflowDefinitionDto>> GetUserWorkflowsAsync(int userId);
    Task<WorkflowDefinitionDto?> GetWorkflowAsync(int workflowId);
    Task<WorkflowDefinitionDto> CreateWorkflowAsync(int userId, WorkflowDefinitionRequest request);
    Task<WorkflowDefinitionDto> UpdateWorkflowAsync(int workflowId, WorkflowDefinitionRequest request);
    Task<WorkflowExecutionDto> ExecuteWorkflowAsync(int workflowId, int conversationId, WorkflowExecutionRequest request);
    Task<List<WorkflowExecutionDto>> GetWorkflowExecutionsAsync(int workflowId, int limit = 50);
}

public interface IUserSegmentationService
{
    Task<UserSegmentDto?> GetUserSegmentAsync(int userId);
    Task<List<UserSegmentDto>> GetSegmentByBehaviorAsync(string behavioralSegment);
    Task<List<UserSegmentDto>> GetChurnRiskUsersAsync(double minRiskScore = 0.7);
    Task<UserSegmentDto> AnalyzeUserSegmentAsync(int userId);
    Task<ChurnPredictionDto> PredictChurnAsync(int userId);
}

public interface ISearchService
{
    Task<SearchResultsPageDto> SearchAsync(int userId, SearchRequest request);
    Task<List<SearchResultDto>> SearchContentAsync(int userId, string query, int skip = 0, int take = 20);
    Task<List<SearchResultDto>> SearchByTopicAsync(int userId, string topic, int skip = 0, int take = 20);
    Task<List<SearchResultDto>> SearchByIntentAsync(int userId, string intent, int skip = 0, int take = 20);
    Task RebuildSearchIndexAsync(int userId);
    Task IndexConversationAsync(int conversationId, int userId, string content, List<string>? topics = null, List<string>? intents = null);
}

public interface IAnalyticsExportService
{
    Task<byte[]> ExportAnalyticsReportAsync(int userId, AnalyticsReportRequest request);
    Task<string> GenerateAnalyticsReportJsonAsync(int userId, AnalyticsReportRequest request);
}
