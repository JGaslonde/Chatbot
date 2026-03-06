using Chatbot.Core.Models.Requests;
using Chatbot.Core.Models.Responses;

namespace Chatbot.Web.Services;

public interface IImportManagementService
{
    Task<ImportJobDto?> StartImportAsync(StartImportRequest request);
    Task<List<ImportJobDto>> GetImportJobsAsync();
    Task<ImportJobDto?> GetImportJobStatusAsync(int jobId);
    Task<bool> UploadChunkAsync(int jobId, byte[] chunk);
    Task<bool> ProcessImportAsync(int jobId);
}
