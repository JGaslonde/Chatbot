using Chatbot.API.Data;
using Chatbot.API.Models.Requests;
using Chatbot.API.Models.Responses;
using Chatbot.API.Services.Core.Interfaces;
using Chatbot.Core.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Chatbot.API.Services.Core;

public class ConversationManagementService : IConversationManagementService
{
    private readonly ChatbotDbContext _context;
    private readonly ILogger<ConversationManagementService> _logger;

    public ConversationManagementService(ChatbotDbContext context, ILogger<ConversationManagementService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PaginatedResponse<ConversationSummaryDto>> GetConversationsAsync(int userId, ConversationFilterRequest filter)
    {
        try
        {
            var query = _context.Conversations.Where(c => c.UserId == userId && c.IsActive);
            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)filter.PageSize);

            var convs = await query
                .OrderByDescending(x => x.StartedAt)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            var items = convs.Select(c => new ConversationSummaryDto
            {
                Title = c.Title ?? "Untitled",
                MessageCount = c.Messages.Count,
                CreatedAt = c.StartedAt,
                UpdatedAt = c.LastMessageAt
            }).ToList();

            return new PaginatedResponse<ConversationSummaryDto>
            {
                Items = items,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalItems = totalItems,
                TotalPages = totalPages
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving conversations");
            throw;
        }
    }

    public async Task<object?> GetConversationDetailAsync(int conversationId, int userId)
        => await _context.Conversations
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.Id == conversationId && c.UserId == userId);

    public async Task<bool> ArchiveConversationAsync(int conversationId, int userId)
    {
        var conv = await _context.Conversations.FirstOrDefaultAsync(c => c.Id == conversationId && c.UserId == userId);
        if (conv == null) return false;
        conv.IsActive = false;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RestoreConversationAsync(int conversationId, int userId)
    {
        var conv = await _context.Conversations.FirstOrDefaultAsync(c => c.Id == conversationId && c.UserId == userId);
        if (conv == null) return false;
        conv.IsActive = true;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteConversationAsync(int conversationId, int userId)
    {
        var conv = await _context.Conversations.FirstOrDefaultAsync(c => c.Id == conversationId && c.UserId == userId);
        if (conv == null) return false;
        conv.IsActive = false;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> PermanentlyDeleteConversationAsync(int conversationId, int userId)
    {
        var conv = await _context.Conversations.FirstOrDefaultAsync(c => c.Id == conversationId && c.UserId == userId);
        if (conv == null) return false;
        _context.Conversations.Remove(conv);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateConversationTagsAsync(int conversationId, List<string> tags, int userId)
    {
        var conv = await _context.Conversations.FirstOrDefaultAsync(c => c.Id == conversationId && c.UserId == userId);
        if (conv == null) return false;
        _logger.LogInformation("Tags: {Tags}", string.Join(",", tags));
        return await Task.FromResult(true);
    }

    public async Task<bool> UpdateConversationStatusAsync(int conversationId, string status, int userId)
    {
        var conv = await _context.Conversations.FirstOrDefaultAsync(c => c.Id == conversationId && c.UserId == userId);
        if (conv == null) return false;
        conv.IsActive = status != "archived";
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateConversationCategoryAsync(int conversationId, string category, int userId)
    {
        var conv = await _context.Conversations.FirstOrDefaultAsync(c => c.Id == conversationId && c.UserId == userId);
        return await Task.FromResult(conv != null);
    }

    public async Task<BulkOperationResultDto> PerformBulkOperationAsync(BulkConversationOperationRequest request, int userId)
    {
        var result = new BulkOperationResultDto { OperationId = Guid.NewGuid().ToString() };
        try
        {
            var convs = await _context.Conversations.Where(c => c.UserId == userId && request.ConversationIds.Contains(c.Id)).ToListAsync();
            foreach (var c in convs)
            {
                if (request.OperationType.ToLower() == "archive") c.IsActive = false;
            }
            await _context.SaveChangesAsync();
            result.Status = "completed";
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Bulk operation error");
            result.Status = "failed";
            return result;
        }
    }

    public async Task<Dictionary<string, int>> GetConversationCountByStatusAsync(int userId)
    {
        var active = await _context.Conversations.CountAsync(c => c.UserId == userId && c.IsActive);
        return new Dictionary<string, int> { { "active", active } };
    }

    public async Task<List<ConversationSummaryDto>> SearchConversationsAsync(int userId, string searchText)
    {
        var convs = await _context.Conversations
            .Where(c => c.UserId == userId && c.IsActive && (c.Title ?? "").Contains(searchText))
            .ToListAsync();

        return convs.Select(c => new ConversationSummaryDto
        {
            Title = c.Title ?? "Untitled",
            MessageCount = c.Messages.Count,
            CreatedAt = c.StartedAt,
            UpdatedAt = c.LastMessageAt
        }).ToList();
    }
}
