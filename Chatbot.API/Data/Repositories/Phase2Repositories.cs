using Chatbot.API.Data.Repositories.Interfaces;
using Chatbot.Core.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Chatbot.API.Data.Repositories;

public class WebhookRepository : Repository<Webhook>, IWebhookRepository
{
    public WebhookRepository(ChatbotDbContext context) : base(context) { }

    public async Task<IEnumerable<Webhook>> GetUserWebhooksAsync(int userId)
    {
        return await _context.Webhooks
            .Where(w => w.UserId == userId)
            .OrderByDescending(w => w.CreatedAt)
            .ToListAsync();
    }

    public async Task<Webhook?> GetWithDeliveriesAsync(int id)
    {
        return await _context.Webhooks
            .Include(w => w.Deliveries)
            .FirstOrDefaultAsync(w => w.Id == id);
    }
}

public class WebhookDeliveryRepository : Repository<WebhookDelivery>, IWebhookDeliveryRepository
{
    public WebhookDeliveryRepository(ChatbotDbContext context) : base(context) { }

    public async Task<IEnumerable<WebhookDelivery>> GetByWebhookIdAsync(int webhookId)
    {
        return await _context.WebhookDeliveries
            .Where(wd => wd.WebhookId == webhookId)
            .OrderByDescending(wd => wd.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<WebhookDelivery>> GetFailedDeliveriesAsync()
    {
        return await _context.WebhookDeliveries
            .Where(wd => wd.Status == WebhookDeliveryStatus.Failed && wd.AttemptCount < 3)
            .OrderBy(wd => wd.CreatedAt)
            .ToListAsync();
    }
}

public class ApiKeyRepository : Repository<ApiKey>, IApiKeyRepository
{
    public ApiKeyRepository(ChatbotDbContext context) : base(context) { }

    public async Task<IEnumerable<ApiKey>> GetUserApiKeysAsync(int userId)
    {
        return await _context.ApiKeys
            .Where(ak => ak.UserId == userId && ak.IsActive)
            .OrderByDescending(ak => ak.CreatedAt)
            .ToListAsync();
    }

    public async Task<ApiKey?> GetByHashAsync(string keyHash)
    {
        return await _context.ApiKeys
            .FirstOrDefaultAsync(ak => ak.KeyHash == keyHash && ak.IsActive);
    }
}

public class TwoFactorAuthRepository : Repository<TwoFactorAuth>, ITwoFactorAuthRepository
{
    public TwoFactorAuthRepository(ChatbotDbContext context) : base(context) { }

    public async Task<TwoFactorAuth?> GetByUserIdAsync(int userId)
    {
        return await _context.TwoFactorAuths
            .FirstOrDefaultAsync(tfa => tfa.UserId == userId);
    }
}

public class IpWhitelistRepository : Repository<IpWhitelist>, IIpWhitelistRepository
{
    public IpWhitelistRepository(ChatbotDbContext context) : base(context) { }

    public async Task<IEnumerable<IpWhitelist>> GetUserWhitelistAsync(int userId)
    {
        return await _context.IpWhitelists
            .Where(iw => iw.UserId == userId && iw.IsActive)
            .OrderByDescending(iw => iw.CreatedAt)
            .ToListAsync();
    }

    public async Task<IpWhitelist?> GetByIpAsync(int userId, string ipAddress)
    {
        return await _context.IpWhitelists
            .FirstOrDefaultAsync(iw => iw.UserId == userId && iw.IpAddress == ipAddress && iw.IsActive);
    }
}

public class ScheduledReportRepository : Repository<ScheduledReport>, IScheduledReportRepository
{
    public ScheduledReportRepository(ChatbotDbContext context) : base(context) { }

    public async Task<IEnumerable<ScheduledReport>> GetUserReportsAsync(int userId)
    {
        return await _context.ScheduledReports
            .Where(sr => sr.UserId == userId)
            .OrderByDescending(sr => sr.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<ScheduledReport>> GetDueReportsAsync()
    {
        var now = DateTime.UtcNow;
        return await _context.ScheduledReports
            .Where(sr => sr.IsActive && sr.NextGeneratedAt <= now)
            .ToListAsync();
    }
}

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
