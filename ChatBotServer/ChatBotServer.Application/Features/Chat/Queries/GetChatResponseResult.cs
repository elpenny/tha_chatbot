namespace ChatBotServer.Application.Features.Chat.Queries;

public class GetChatResponseResult
{
    public string Content { get; set; } = string.Empty;
    public bool IsComplete { get; set; }
}