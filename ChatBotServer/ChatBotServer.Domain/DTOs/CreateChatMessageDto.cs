namespace ChatBotServer.Domain.DTOs;

public class CreateChatMessageDto
{
    public string Content { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public int ConversationId { get; set; }
}