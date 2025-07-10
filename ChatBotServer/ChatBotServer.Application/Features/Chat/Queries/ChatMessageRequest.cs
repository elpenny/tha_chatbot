namespace ChatBotServer.Application.Features.Chat.Queries;

public class ChatMessageRequest
{
    public string Content { get; set; } = string.Empty;
    public int? ConversationId { get; set; }
}