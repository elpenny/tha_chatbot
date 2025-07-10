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

public class CreateChatConversationDto
{
    public string Title { get; set; } = string.Empty;
    public string? UserId { get; set; }
}

public class ChatResponseDto
{
    public int ConversationId { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool IsComplete { get; set; }
    public int MessageId { get; set; }
}