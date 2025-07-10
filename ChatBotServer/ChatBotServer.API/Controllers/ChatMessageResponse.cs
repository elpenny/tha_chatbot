namespace ChatBotServer.API.Controllers;

public class ChatMessageResponse
{
    public string Content { get; set; } = string.Empty;
    public bool IsComplete { get; set; }
}