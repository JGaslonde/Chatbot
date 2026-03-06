using Chatbot.Core.Models.Requests;
using System.ComponentModel.DataAnnotations;

namespace Chatbot.Core.Models.Entities;

/// <summary>
/// Represents a saved search with filters for quick access to frequently used queries
/// </summary>
public class SavedSearch
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// JSON serialized SearchConversationsRequest for search parameters
    /// </summary>
    [Required]
    public string SearchParams { get; set; } = string.Empty;

    /// <summary>
    /// Whether this is marked as a favorite/pinned search
    /// </summary>
    public bool IsFavorite { get; set; }

    /// <summary>
    /// Number of times this saved search has been used
    /// </summary>
    public int UsageCount { get; set; }

    /// <summary>
    /// When the saved search was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When the saved search was last modified
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When the saved search was last used
    /// </summary>
    public DateTime? LastUsedAt { get; set; }

    // Foreign key
    public virtual User? User { get; set; }
}
