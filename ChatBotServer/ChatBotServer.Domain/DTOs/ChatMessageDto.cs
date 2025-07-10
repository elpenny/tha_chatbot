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

public class CreateChatMessageDto
{
    public string Content { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public int ConversationId { get; set; }
}

public class UpdateMessageRatingDto
{
    public int MessageId { get; set; }
    public int? Rating { get; set; } // 1 for thumbs up, -1 for thumbs down, null to remove rating
}