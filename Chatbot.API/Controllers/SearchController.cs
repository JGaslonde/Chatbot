using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Chatbot.API.Services.Phase3.Interfaces;
using Chatbot.API.Services.Analysis;
using Chatbot.Core.Models.Requests;
using Chatbot.Core.Models.Responses;

namespace Chatbot.API.Controllers;

/// <summary>
/// Phase 3 Advanced Search API endpoints
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class SearchController : ControllerBase
{
    private readonly ISearchService _searchService;
    private readonly SavedSearchService _savedSearchService;
    private readonly ILogger<SearchController> _logger;

    public SearchController(ISearchService searchService, SavedSearchService savedSearchService, ILogger<SearchController> logger)
    {
        _searchService = searchService;
        _savedSearchService = savedSearchService;
        _logger = logger;
    }

    /// <summary>
    /// Extract userId from JWT claims
    /// </summary>
    private int GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out var userId) ? userId : 0;
    }

    /// <summary>
    /// Advanced search across conversations
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<SearchResultsPageDto>> Search([FromBody] SearchRequest request)
    {
        try
        {
            var userId = GetUserId();
            var results = await _searchService.SearchAsync(userId, request);
            return Ok(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching");
            return StatusCode(500, new { error = "Search failed" });
        }
    }

    /// <summary>
    /// Search by content
    /// </summary>
    [HttpGet("content")]
    public async Task<ActionResult<List<SearchResultDto>>> SearchContent(
        [FromQuery] string query,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 20)
    {
        try
        {
            var userId = GetUserId();
            var results = await _searchService.SearchContentAsync(userId, query, skip, take);
            return Ok(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching content");
            return StatusCode(500, new { error = "Search failed" });
        }
    }

    /// <summary>
    /// Search by topic
    /// </summary>
    [HttpGet("topics/{topic}")]
    public async Task<ActionResult<List<SearchResultDto>>> SearchByTopic(
        string topic,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 20)
    {
        try
        {
            var userId = GetUserId();
            var results = await _searchService.SearchByTopicAsync(userId, topic, skip, take);
            return Ok(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching by topic");
            return StatusCode(500, new { error = "Search failed" });
        }
    }

    /// <summary>
    /// Search by intent
    /// </summary>
    [HttpGet("intents/{intent}")]
    public async Task<ActionResult<List<SearchResultDto>>> SearchByIntent(
        string intent,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 20)
    {
        try
        {
            var userId = GetUserId();
            var results = await _searchService.SearchByIntentAsync(userId, intent, skip, take);
            return Ok(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching by intent");
            return StatusCode(500, new { error = "Search failed" });
        }
    }

    /// <summary>
    /// Rebuild search index for user
    /// </summary>
    [HttpPost("rebuild-index")]
    public async Task<ActionResult> RebuildSearchIndex()
    {
        try
        {
            var userId = GetUserId();
            await _searchService.RebuildSearchIndexAsync(userId);
            return Accepted(new { message = "Index rebuild in progress" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rebuilding search index");
            return StatusCode(500, new { error = "Index rebuild failed" });
        }
    }

    // ========== SAVED SEARCHES ==========

    /// <summary>
    /// Get all saved searches for the current user
    /// </summary>
    [HttpGet("saved-searches")]
    public async Task<ActionResult<List<SavedSearchDto>>> GetSavedSearches()
    {
        try
        {
            var userId = GetUserId();
            var searches = await _savedSearchService.GetUserSavedSearchesAsync(userId);
            return Ok(searches);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving saved searches");
            return StatusCode(500, new { error = "Failed to retrieve saved searches" });
        }
    }

    /// <summary>
    /// Create a new saved search
    /// </summary>
    [HttpPost("saved-searches")]
    public async Task<ActionResult<SavedSearchDto>> CreateSavedSearch([FromBody] CreateSavedSearchRequest request)
    {
        try
        {
            var userId = GetUserId();
            var savedSearch = await _savedSearchService.CreateSavedSearchAsync(userId, request);
            return CreatedAtAction(nameof(GetSavedSearch), new { searchId = savedSearch.Id }, savedSearch);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating saved search");
            return StatusCode(500, new { error = "Failed to create saved search" });
        }
    }

    /// <summary>
    /// Get a specific saved search
    /// </summary>
    [HttpGet("saved-searches/{searchId}")]
    public async Task<ActionResult<SavedSearchDto>> GetSavedSearch(int searchId)
    {
        try
        {
            var userId = GetUserId();
            var searches = await _savedSearchService.GetUserSavedSearchesAsync(userId);
            var search = searches.FirstOrDefault(s => s.Id == searchId);
            if (search == null)
                return NotFound(new { error = "Saved search not found" });
            return Ok(search);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving saved search");
            return StatusCode(500, new { error = "Failed to retrieve saved search" });
        }
    }

    /// <summary>
    /// Update a saved search
    /// </summary>
    [HttpPut("saved-searches/{searchId}")]
    public async Task<ActionResult<SavedSearchDto>> UpdateSavedSearch(int searchId, [FromBody] UpdateSavedSearchRequest request)
    {
        try
        {
            var userId = GetUserId();
            var updated = await _savedSearchService.UpdateSavedSearchAsync(userId, searchId, request);
            if (updated == null)
                return NotFound(new { error = "Saved search not found" });
            return Ok(updated);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating saved search");
            return StatusCode(500, new { error = "Failed to update saved search" });
        }
    }

    /// <summary>
    /// Delete a saved search
    /// </summary>
    [HttpDelete("saved-searches/{searchId}")]
    public async Task<ActionResult> DeleteSavedSearch(int searchId)
    {
        try
        {
            var userId = GetUserId();
            var deleted = await _savedSearchService.DeleteSavedSearchAsync(userId, searchId);
            if (!deleted)
                return NotFound(new { error = "Saved search not found" });
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting saved search");
            return StatusCode(500, new { error = "Failed to delete saved search" });
        }
    }

    /// <summary>
    /// Execute a saved search by ID
    /// </summary>
    [HttpPost("saved-searches/{searchId}/execute")]
    public async Task<ActionResult<SearchResultsPageDto>> ExecuteSavedSearch(int searchId)
    {
        try
        {
            var userId = GetUserId();
            var searchParams = await _savedSearchService.UseSavedSearchAsync(userId, searchId);
            if (searchParams == null)
                return NotFound(new { error = "Saved search not found" });

            // Return the search params - client can use to perform search
            return Ok(new { data = searchParams, message = "Saved search retrieved and usage updated" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing saved search");
            return StatusCode(500, new { error = "Failed to execute saved search" });
        }
    }

    /// <summary>
    /// Get search suggestions based on saved searches
    /// </summary>
    [HttpGet("suggestions")]
    public async Task<ActionResult<List<string>>> GetSearchSuggestions([FromQuery] string query, [FromQuery] int limit = 10)
    {
        try
        {
            var userId = GetUserId();
            var suggestions = await _savedSearchService.GetSearchSuggestionsAsync(userId, query, limit);
            return Ok(suggestions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting search suggestions");
            return StatusCode(500, new { error = "Failed to get search suggestions" });
        }
    }
}
