namespace ChatBotServer.Application.Features.Chat.Results;

public class StreamingChatResult
{
    public int ConversationId { get; set; }
    public IAsyncEnumerable<string> ContentStream { get; set; } = null!;
}