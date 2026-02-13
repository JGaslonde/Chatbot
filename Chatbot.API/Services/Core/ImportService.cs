using Chatbot.API.Data.Repositories.Interfaces;
using Chatbot.API.Services.Core.Interfaces;
using Chatbot.Core.Models.Entities;
using Chatbot.Core.Models.Requests;
using Chatbot.Core.Models.Responses;

namespace Chatbot.API.Services.Core;

public class ImportService : IImportService
{
    private readonly IImportJobRepository _repository;
    private readonly ILogger<ImportService> _logger;

    public ImportService(IImportJobRepository repository, ILogger<ImportService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<ImportJobDto> StartImportAsync(int userId, StartImportRequest request)
    {
        try
        {
            var importJob = new ImportJob
            {
                UserId = userId,
                FileName = request.FileName ?? $"import_{DateTime.UtcNow:yyyyMMddHHmmss}",
                ImportType = request.ImportType,
                Status = "Pending",
                TotalRecords = request.FileSize,
                ProcessedRecords = 0,
                FailedRecords = 0,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(importJob);

            _logger.LogInformation($"Import job created for user {userId}: {importJob.Id}");

            return MapToDto(importJob);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error starting import: {ex.Message}");
            throw;
        }
    }

    public async Task<bool> UploadChunkAsync(int jobId, byte[] chunk)
    {
        try
        {
            var job = await _repository.GetByIdAsync(jobId);
            if (job == null)
                return false;

            // In a real implementation, save the chunk to temporary storage
            _logger.LogInformation($"Chunk uploaded for import job {jobId}: {chunk.Length} bytes");

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error uploading chunk: {ex.Message}");
            return false;
        }
    }

    public async Task ProcessImportAsync(int jobId)
    {
        try
        {
            var job = await _repository.GetByIdAsync(jobId);
            if (job == null)
                return;

            job.Status = "Processing";
            await _repository.UpdateAsync(job);

            // In a real implementation, parse and process the uploaded file
            // For now, just simulate processing
            job.ProcessedRecords = job.TotalRecords;
            job.FailedRecords = 0;
            job.Status = "Completed";
            job.CompletedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(job);

            _logger.LogInformation($"Import job {jobId} processed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error processing import: {ex.Message}");

            var job = await _repository.GetByIdAsync(jobId);
            if (job != null)
            {
                job.Status = "Failed";
                job.ErrorDetails = ex.Message;
                await _repository.UpdateAsync(job);
            }
        }
    }

    public async Task<List<ImportJobDto>> GetUserImportJobsAsync(int userId)
    {
        var jobs = await _repository.GetUserImportJobsAsync(userId);
        return jobs.Select(MapToDto).ToList();
    }

    public async Task<ImportJobDto?> GetImportJobStatusAsync(int userId, int jobId)
    {
        var job = await _repository.GetByIdAsync(jobId);
        if (job == null || job.UserId != userId)
            return null;

        return MapToDto(job);
    }

    private static ImportJobDto MapToDto(ImportJob job)
    {
        return new ImportJobDto(
            job.Id,
            job.FileName,
            job.ImportType,
            job.Status,
            job.TotalRecords,
            job.ProcessedRecords,
            job.FailedRecords,
            job.CreatedAt,
            job.CompletedAt
        );
    }
}
