namespace ChatBotServer.Domain.Entities;

public class ChatConversation
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation property
    public List<ChatMessage> Messages { get; set; } = new();
}