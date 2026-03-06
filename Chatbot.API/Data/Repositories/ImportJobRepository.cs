using Chatbot.API.Data.Repositories.Interfaces;
using Chatbot.Core.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Chatbot.API.Data.Repositories;

public class ImportJobRepository : Repository<ImportJob>, IImportJobRepository
{
    public ImportJobRepository(ChatbotDbContext context) : base(context) { }

    public async Task<IEnumerable<ImportJob>> GetUserImportJobsAsync(int userId)
    {
        return await _context.ImportJobs
            .Where(ij => ij.UserId == userId)
            .OrderByDescending(ij => ij.CreatedAt)
            .ToListAsync();
    }

    public async Task<ImportJob?> GetIncludingDetailsAsync(int id)
    {
        return await _context.ImportJobs
            .AsNoTracking()
            .FirstOrDefaultAsync(ij => ij.Id == id);
    }
}
