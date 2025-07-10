namespace ChatBotServer.Domain.DTOs;

public class ChatResponseDto
{
    public int ConversationId { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool IsComplete { get; set; }
    public int MessageId { get; set; }
}