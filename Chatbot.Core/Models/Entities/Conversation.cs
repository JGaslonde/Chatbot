namespace Chatbot.Core.Models.Entities;

public class Conversation
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string? Title { get; set; }
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastMessageAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
    public string? Summary { get; set; }

    // Navigation
    public required User User { get; set; }
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}
