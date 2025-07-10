namespace ChatBotServer.Domain.DTOs;

public class ChatConversationDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UserId { get; set; }
    public List<ChatMessageDto> Messages { get; set; } = new();
}