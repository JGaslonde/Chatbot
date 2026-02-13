using Chatbot.Core.Models.Entities;

namespace Chatbot.API.Data.Repositories.Interfaces;

public interface IWebhookRepository : IRepository<Webhook>
{
    Task<IEnumerable<Webhook>> GetUserWebhooksAsync(int userId);
    Task<Webhook?> GetWithDeliveriesAsync(int id);
}

public interface IWebhookDeliveryRepository : IRepository<WebhookDelivery>
{
    Task<IEnumerable<WebhookDelivery>> GetByWebhookIdAsync(int webhookId);
    Task<IEnumerable<WebhookDelivery>> GetFailedDeliveriesAsync();
}

public interface IApiKeyRepository : IRepository<ApiKey>
{
    Task<IEnumerable<ApiKey>> GetUserApiKeysAsync(int userId);
    Task<ApiKey?> GetByHashAsync(string keyHash);
}

public interface ITwoFactorAuthRepository : IRepository<TwoFactorAuth>
{
    Task<TwoFactorAuth?> GetByUserIdAsync(int userId);
}

public interface IIpWhitelistRepository : IRepository<IpWhitelist>
{
    Task<IEnumerable<IpWhitelist>> GetUserWhitelistAsync(int userId);
    Task<IpWhitelist?> GetByIpAsync(int userId, string ipAddress);
}

public interface IScheduledReportRepository : IRepository<ScheduledReport>
{
    Task<IEnumerable<ScheduledReport>> GetUserReportsAsync(int userId);
    Task<IEnumerable<ScheduledReport>> GetDueReportsAsync();
}

public interface IImportJobRepository : IRepository<ImportJob>
{
    Task<IEnumerable<ImportJob>> GetUserImportJobsAsync(int userId);
    Task<ImportJob?> GetIncludingDetailsAsync(int id);
}
