using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chatbot.Core.Models.Entities;

/// <summary>
/// Full-text search index entries for conversations.
/// </summary>
public class SearchIndex
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public int ConversationId { get; set; }

    /// <summary>
    /// The searchable content (combined user messages)
    /// </summary>
    [Required]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Extracted keywords from conversation
    /// </summary>
    public string? Keywords { get; set; }

    /// <summary>
    /// Topics detected in conversation
    /// </summary>
    public string? Topics { get; set; }

    /// <summary>
    /// Intents detected in conversation
    /// </summary>
    public string? Intents { get; set; }

    /// <summary>
    /// Full-text search vector (for advanced search engines)
    /// </summary>
    public string? SearchVector { get; set; }

    /// <summary>
    /// Relevance score for search ranking
    /// </summary>
    public double RelevanceScore { get; set; }

    [Required]
    public DateTime IndexedAt { get; set; }

    public DateTime? LastSearchedAt { get; set; }
}
