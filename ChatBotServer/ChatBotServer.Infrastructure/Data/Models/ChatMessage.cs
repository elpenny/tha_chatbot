namespace ChatBotServer.Infrastructure.Data.Models
{
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

        // Foreign key
        public int ConversationId { get; set; }

        // Navigation property
        public ChatConversation Conversation { get; set; } = null!;
    }
}
