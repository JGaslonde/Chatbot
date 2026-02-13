using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chatbot.Core.Models.Entities;

/// <summary>
/// Notification entity for real-time updates.
/// </summary>
[Table("Notifications")]
public class Notification
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    [MaxLength(50)]
    public string Type { get; set; } = "message";

    [Required]
    [MaxLength(500)]
    public string Message { get; set; } = string.Empty;

    public int? RelatedConversationId { get; set; }

    public bool IsRead { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ReadAt { get; set; }

    // Navigation properties
    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }

    [ForeignKey(nameof(RelatedConversationId))]
    public Conversation? RelatedConversation { get; set; }
}
