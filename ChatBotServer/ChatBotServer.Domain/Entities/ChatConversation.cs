namespace ChatBotServer.Domain.Entities;

public class ChatConversation
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public string? UserId { get; set; }

    // Navigation property
    public ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
}