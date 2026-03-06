using Chatbot.Core.Models.Entities;

namespace Chatbot.API.Data.Repositories.Interfaces;

public interface IImportJobRepository : IRepository<ImportJob>
{
    Task<IEnumerable<ImportJob>> GetUserImportJobsAsync(int userId);
    Task<ImportJob?> GetIncludingDetailsAsync(int id);
}
