namespace ChatBotServer.Domain.DTOs;

public class CreateChatConversationDto
{
    public string Title { get; set; } = string.Empty;
    public string? UserId { get; set; }
}