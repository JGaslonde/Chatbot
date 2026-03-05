using Chatbot.API.Data.Context;
using Chatbot.Core.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Chatbot.API.Services.Processing;

public interface IKnowledgeBaseService
{
    Task<KnowledgeEntry?> FindBestMatchAsync(string query);
}

public class KnowledgeBaseService : IKnowledgeBaseService
{
    private readonly ChatbotDbContext _db;
    private readonly ILogger<KnowledgeBaseService> _logger;

    public KnowledgeBaseService(ChatbotDbContext db, ILogger<KnowledgeBaseService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<KnowledgeEntry?> FindBestMatchAsync(string query)
    {
        var entries = await _db.KnowledgeEntries
            .AsNoTracking()
            .Where(k => k.IsActive)
            .ToListAsync();

        if (entries.Count == 0) return null;

        var queryTokens = Tokenize(query);
        KnowledgeEntry? best = null;
        int bestScore = 0;

        foreach (var entry in entries)
        {
            var score = ScoreEntry(queryTokens, entry);
            if (score > bestScore)
            {
                bestScore = score;
                best = entry;
            }
        }

        // Only return a match if confidence is above threshold
        if (bestScore < 2)
        {
            _logger.LogDebug("No confident knowledge base match for query (best score: {Score})", bestScore);
            return null;
        }

        _logger.LogInformation("Knowledge base matched entry {Id} (score: {Score})", best!.Id, bestScore);
        return best;
    }

    private static int ScoreEntry(HashSet<string> queryTokens, KnowledgeEntry entry)
    {
        var score = 0;

        // Score against keywords (highest weight)
        if (!string.IsNullOrWhiteSpace(entry.Keywords))
        {
            var keywords = entry.Keywords
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(k => k.ToLowerInvariant());

            foreach (var keyword in keywords)
                if (queryTokens.Contains(keyword)) score += 3;
        }

        // Score against question tokens
        var questionTokens = Tokenize(entry.Question);
        score += queryTokens.Intersect(questionTokens).Count();

        return score;
    }

    private static HashSet<string> Tokenize(string text)
    {
        return text.ToLowerInvariant()
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(t => t.Length > 2)
            .ToHashSet();
    }
}
