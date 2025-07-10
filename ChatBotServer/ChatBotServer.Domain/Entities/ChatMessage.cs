namespace ChatBotServer.Domain.Entities;

public enum MessageRole
{
    User,
    Assistant,
    System
}

public class ChatMessage
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public MessageRole Role { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? Rating { get; set; } // 1 for thumbs up, -1 for thumbs down, null for no rating

    // Foreign key
    public int ConversationId { get; set; }

    // Navigation property
    public ChatConversation Conversation { get; set; } = null!;
}