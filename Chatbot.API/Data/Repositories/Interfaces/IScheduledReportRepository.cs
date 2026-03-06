using Chatbot.Core.Models.Entities;

namespace Chatbot.API.Data.Repositories.Interfaces;

public interface IScheduledReportRepository : IRepository<ScheduledReport>
{
    Task<IEnumerable<ScheduledReport>> GetUserReportsAsync(int userId);
    Task<IEnumerable<ScheduledReport>> GetDueReportsAsync();
}
