using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chatbot.Core.Models.Entities;

/// <summary>
/// Audit log entity for tracking user actions.
/// </summary>
[Table("AuditLogs")]
public class AuditLog
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    [MaxLength(50)]
    public string Action { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string ResourceType { get; set; } = string.Empty;

    public int? ResourceId { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    [MaxLength(50)]
    public string? IpAddress { get; set; }

    public string? Details { get; set; } // JSON serialized details

    // Navigation property
    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }
}
