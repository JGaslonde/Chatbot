using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Chatbot.API.Controllers.Chat;
using Chatbot.API.Data.Context;
using Chatbot.API.Infrastructure.Auth;
using Chatbot.API.Infrastructure.Http;
using Chatbot.Core.Models.Entities;

namespace Chatbot.API.Controllers.Admin;

[Authorize(Policy = "AdminOnly")]
[Route("api/admin")]
public class AdminController : ApiControllerBase
{
    private readonly ChatbotDbContext _db;

    public AdminController(
        ChatbotDbContext db,
        IUserContextProvider userContextProvider,
        IApiResponseBuilder responseBuilder,
        ILogger<AdminController> logger)
        : base(userContextProvider, responseBuilder, logger)
    {
        _db = db;
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        LogAction("AdminGetUsers", new { page, pageSize });

        var total = await _db.Users.CountAsync();
        var users = await _db.Users
            .AsNoTracking()
            .OrderByDescending(u => u.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new
            {
                u.Id,
                u.Username,
                u.Email,
                u.DisplayName,
                u.Role,
                u.IsActive,
                u.CreatedAt,
                u.LastActive,
                ConversationCount = u.Conversations.Count
            })
            .ToListAsync();

        return Ok(new { Total = total, Page = page, PageSize = pageSize, Users = users }, "Users retrieved");
    }

    [HttpPut("users/{id}/role")]
    public async Task<IActionResult> UpdateUserRole(int id, [FromBody] UpdateUserRoleRequest request)
    {
        LogAction("AdminUpdateUserRole", new { userId = id, newRole = request.Role });

        var user = await _db.Users.FindAsync(id);
        if (user == null) return NotFound(new { message = "User not found" });

        user.Role = request.Role;
        await _db.SaveChangesAsync();

        return Ok(new { user.Id, user.Username, user.Role }, "Role updated");
    }

    [HttpPut("users/{id}/deactivate")]
    public async Task<IActionResult> DeactivateUser(int id)
    {
        LogAction("AdminDeactivateUser", new { userId = id });

        var user = await _db.Users.FindAsync(id);
        if (user == null) return NotFound(new { message = "User not found" });

        user.IsActive = false;
        await _db.SaveChangesAsync();

        return Ok(new { user.Id, user.Username, user.IsActive }, "User deactivated");
    }

    [HttpPut("users/{id}/activate")]
    public async Task<IActionResult> ActivateUser(int id)
    {
        LogAction("AdminActivateUser", new { userId = id });

        var user = await _db.Users.FindAsync(id);
        if (user == null) return NotFound(new { message = "User not found" });

        user.IsActive = true;
        await _db.SaveChangesAsync();

        return Ok(new { user.Id, user.Username, user.IsActive }, "User activated");
    }

    [HttpGet("conversations")]
    public async Task<IActionResult> GetAllConversations([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        LogAction("AdminGetConversations", new { page, pageSize });

        var total = await _db.Conversations.CountAsync();
        var conversations = await _db.Conversations
            .AsNoTracking()
            .Include(c => c.User)
            .OrderByDescending(c => c.LastMessageAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new
            {
                c.Id,
                c.Title,
                c.StartedAt,
                c.LastMessageAt,
                c.IsActive,
                c.Summary,
                Owner = c.User.Username,
                MessageCount = c.Messages.Count
            })
            .ToListAsync();

        return Ok(new { Total = total, Page = page, PageSize = pageSize, Conversations = conversations }, "Conversations retrieved");
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetSystemStats()
    {
        LogAction("AdminGetStats");

        var stats = new
        {
            TotalUsers = await _db.Users.CountAsync(),
            ActiveUsers = await _db.Users.CountAsync(u => u.IsActive),
            TotalConversations = await _db.Conversations.CountAsync(),
            ActiveConversations = await _db.Conversations.CountAsync(c => c.IsActive),
            TotalMessages = await _db.Messages.CountAsync(),
            OpenTickets = await _db.EscalationTickets.CountAsync(t => t.Status == TicketStatus.Open),
            TotalKnowledgeEntries = await _db.KnowledgeEntries.CountAsync(k => k.IsActive),
            MessagesSentToday = await _db.Messages.CountAsync(m => m.SentAt.Date == DateTime.UtcNow.Date)
        };

        return Ok(stats, "System stats retrieved");
    }

    [HttpGet("tickets")]
    public async Task<IActionResult> GetEscalationTickets([FromQuery] TicketStatus? status = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        LogAction("AdminGetTickets", new { status, page, pageSize });

        var query = _db.EscalationTickets
            .AsNoTracking()
            .Include(t => t.User)
            .Include(t => t.Conversation)
            .AsQueryable();

        if (status.HasValue)
            query = query.Where(t => t.Status == status.Value);

        var total = await query.CountAsync();
        var tickets = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new
            {
                t.Id,
                t.Reason,
                t.Status,
                t.CreatedAt,
                t.ResolvedAt,
                t.Resolution,
                UserName = t.User.Username,
                ConversationTitle = t.Conversation.Title
            })
            .ToListAsync();

        return Ok(new { Total = total, Page = page, PageSize = pageSize, Tickets = tickets }, "Tickets retrieved");
    }

    [HttpPut("tickets/{id}/resolve")]
    public async Task<IActionResult> ResolveTicket(int id, [FromBody] ResolveTicketRequest request)
    {
        LogAction("AdminResolveTicket", new { ticketId = id });

        var ticket = await _db.EscalationTickets.FindAsync(id);
        if (ticket == null) return NotFound(new { message = "Ticket not found" });

        ticket.Status = TicketStatus.Resolved;
        ticket.Resolution = request.Resolution;
        ticket.ResolvedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return Ok(new { ticket.Id, ticket.Status, ticket.ResolvedAt }, "Ticket resolved");
    }

    // Knowledge base management
    [HttpGet("knowledge")]
    public async Task<IActionResult> GetKnowledgeEntries([FromQuery] string? category = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var query = _db.KnowledgeEntries.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(k => k.Category == category);

        var total = await query.CountAsync();
        var entries = await query
            .OrderBy(k => k.Category)
            .ThenBy(k => k.Question)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(new { Total = total, Page = page, PageSize = pageSize, Entries = entries }, "Knowledge entries retrieved");
    }

    [HttpPost("knowledge")]
    public async Task<IActionResult> CreateKnowledgeEntry([FromBody] KnowledgeEntry entry)
    {
        entry.CreatedAt = DateTime.UtcNow;
        entry.UpdatedAt = DateTime.UtcNow;
        _db.KnowledgeEntries.Add(entry);
        await _db.SaveChangesAsync();
        return Ok(entry, "Knowledge entry created");
    }

    [HttpPut("knowledge/{id}")]
    public async Task<IActionResult> UpdateKnowledgeEntry(int id, [FromBody] KnowledgeEntry updated)
    {
        var entry = await _db.KnowledgeEntries.FindAsync(id);
        if (entry == null) return NotFound(new { message = "Entry not found" });

        entry.Category = updated.Category;
        entry.Question = updated.Question;
        entry.Answer = updated.Answer;
        entry.Keywords = updated.Keywords;
        entry.IsActive = updated.IsActive;
        entry.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return Ok(entry, "Knowledge entry updated");
    }

    [HttpDelete("knowledge/{id}")]
    public async Task<IActionResult> DeleteKnowledgeEntry(int id)
    {
        var entry = await _db.KnowledgeEntries.FindAsync(id);
        if (entry == null) return NotFound(new { message = "Entry not found" });

        entry.IsActive = false;
        entry.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return Ok(new { id }, "Knowledge entry deactivated");
    }
}

public record UpdateUserRoleRequest(UserRole Role);
public record ResolveTicketRequest(string Resolution);
