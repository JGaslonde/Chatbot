using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chatbot.Core.Models.Entities;

/// <summary>
/// User notification preferences entity.
/// </summary>
[Table("UserNotificationPreferences")]
public class UserNotificationPreferences
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    public bool EnableNotifications { get; set; } = true;

    public bool NotifyOnNewMessage { get; set; } = true;

    public bool NotifyOnConversationUpdate { get; set; } = true;

    public bool NotifyOnAnalyticsUpdate { get; set; } = false;

    [MaxLength(50)]
    public string NotificationFrequency { get; set; } = "immediate";

    public DateTime LastModified { get; set; } = DateTime.UtcNow;

    // Navigation property
    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }
}
