using Chatbot.Core.Models.Requests;
using Chatbot.Core.Models.Responses;
using System.Text.Json;

namespace Chatbot.API.Services.Analysis;

/// <summary>
/// Service for managing saved searches for quick access to frequently used queries
/// </summary>
public class SavedSearchService
{
    private readonly ILogger<SavedSearchService> _logger;
    // In-memory storage for demo - replace with database in production
    private readonly Dictionary<int, List<SavedSearchDto>> _savedSearches = new();

    public SavedSearchService(ILogger<SavedSearchService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get all saved searches for a user
    /// </summary>
    public async Task<List<SavedSearchDto>> GetUserSavedSearchesAsync(int userId)
    {
        return await Task.FromResult(
            _savedSearches.ContainsKey(userId)
                ? _savedSearches[userId].OrderByDescending(s => s.IsFavorite).ThenByDescending(s => s.LastUsedAt).ToList()
                : new List<SavedSearchDto>());
    }

    /// <summary>
    /// Create a new saved search
    /// </summary>
    public async Task<SavedSearchDto> CreateSavedSearchAsync(int userId, CreateSavedSearchRequest request)
    {
        try
        {
            var savedSearch = new SavedSearchDto
            {
                Id = await GenerateIdAsync(userId),
                UserId = userId,
                Name = request.Name,
                Description = request.Description ?? string.Empty,
                SearchParams = JsonSerializer.Serialize(request.SearchParams),
                IsFavorite = false,
                UsageCount = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                LastUsedAt = null
            };

            if (!_savedSearches.ContainsKey(userId))
            {
                _savedSearches[userId] = new List<SavedSearchDto>();
            }

            _savedSearches[userId].Add(savedSearch);
            _logger.LogInformation($"Saved search '{request.Name}' created for user {userId}");

            return await Task.FromResult(savedSearch);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error creating saved search for user {userId}");
            throw;
        }
    }

    /// <summary>
    /// Update an existing saved search
    /// </summary>
    public async Task<SavedSearchDto?> UpdateSavedSearchAsync(int userId, int searchId, UpdateSavedSearchRequest request)
    {
        try
        {
            if (!_savedSearches.ContainsKey(userId))
                return null;

            var search = _savedSearches[userId].FirstOrDefault(s => s.Id == searchId);
            if (search == null)
                return null;

            if (!string.IsNullOrEmpty(request.Name))
                search.Name = request.Name;
            if (request.Description != null)
                search.Description = request.Description;
            if (request.SearchParams != null)
                search.SearchParams = JsonSerializer.Serialize(request.SearchParams);
            if (request.IsFavorite.HasValue)
                search.IsFavorite = request.IsFavorite.Value;

            search.UpdatedAt = DateTime.UtcNow;

            _logger.LogInformation($"Saved search {searchId} updated for user {userId}");
            return await Task.FromResult(search);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating saved search {searchId} for user {userId}");
            throw;
        }
    }

    /// <summary>
    /// Delete a saved search
    /// </summary>
    public async Task<bool> DeleteSavedSearchAsync(int userId, int searchId)
    {
        try
        {
            if (!_savedSearches.ContainsKey(userId))
                return false;

            var removed = _savedSearches[userId].RemoveAll(s => s.Id == searchId) > 0;
            if (removed)
                _logger.LogInformation($"Saved search {searchId} deleted for user {userId}");

            return await Task.FromResult(removed);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting saved search {searchId} for user {userId}");
            throw;
        }
    }

    /// <summary>
    /// Use a saved search and increment its usage count
    /// </summary>
    public async Task<SearchConversationsRequest?> UseSavedSearchAsync(int userId, int searchId)
    {
        try
        {
            if (!_savedSearches.ContainsKey(userId))
                return null;

            var search = _savedSearches[userId].FirstOrDefault(s => s.Id == searchId);
            if (search == null)
                return null;

            search.UsageCount++;
            search.LastUsedAt = DateTime.UtcNow;

            var searchParams = JsonSerializer.Deserialize<SearchConversationsRequest>(search.SearchParams);
            _logger.LogInformation($"Saved search {searchId} used by user {userId} (usage count: {search.UsageCount})");

            return await Task.FromResult(searchParams);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error using saved search {searchId} for user {userId}");
            throw;
        }
    }

    /// <summary>
    /// Get suggested searches based on user's search history
    /// </summary>
    public async Task<List<string>> GetSearchSuggestionsAsync(int userId, string query, int limit = 10)
    {
        try
        {
            var suggestions = new List<string>();

            if (!_savedSearches.ContainsKey(userId))
                return suggestions;

            // Return saved searches that match the query prefix
            suggestions = _savedSearches[userId]
                .Where(s => s.Name.StartsWith(query, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(s => s.UsageCount)
                .ThenByDescending(s => s.LastUsedAt)
                .Select(s => s.Name)
                .Take(limit)
                .ToList();

            return await Task.FromResult(suggestions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting search suggestions for user {userId}");
            return new List<string>();
        }
    }

    private async Task<int> GenerateIdAsync(int userId)
    {
        return await Task.FromResult(
            _savedSearches.ContainsKey(userId)
                ? _savedSearches[userId].Max(s => s.Id) + 1
                : 1);
    }
}

/// <summary>
/// Data Transfer Object for saved searches
/// </summary>
public class SavedSearchDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string SearchParams { get; set; } = string.Empty;
    public bool IsFavorite { get; set; }
    public int UsageCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? LastUsedAt { get; set; }
}

/// <summary>
/// Request to create a new saved search
/// </summary>
public class CreateSavedSearchRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public SearchConversationsRequest SearchParams { get; set; } = new();
}

/// <summary>
/// Request to update an existing saved search
/// </summary>
public class UpdateSavedSearchRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public SearchConversationsRequest? SearchParams { get; set; }
    public bool? IsFavorite { get; set; }
}
