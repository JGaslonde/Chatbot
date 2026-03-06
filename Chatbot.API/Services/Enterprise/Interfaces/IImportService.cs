using Chatbot.Core.Models.Requests;
using Chatbot.Core.Models.Responses;

namespace Chatbot.API.Services.Core.Interfaces;

/// <summary>
/// Service for bulk import operations.
/// </summary>
public interface IImportService
{
    Task<ImportJobDto> StartImportAsync(int userId, StartImportRequest request);
    Task<bool> UploadChunkAsync(int jobId, byte[] chunk);
    Task ProcessImportAsync(int jobId);
    Task<List<ImportJobDto>> GetUserImportJobsAsync(int userId);
    Task<ImportJobDto?> GetImportJobStatusAsync(int userId, int jobId);
}
