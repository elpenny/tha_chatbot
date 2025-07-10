namespace ChatBotServer.Domain.DTOs;

public class ChatMessageDto
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int? Rating { get; set; }
    public int ConversationId { get; set; }
}