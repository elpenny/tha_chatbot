namespace ChatBotServer.API.Controllers;

public class ChatMessageRequest
{
    public string Content { get; set; } = string.Empty;
    public int? ConversationId { get; set; }
}