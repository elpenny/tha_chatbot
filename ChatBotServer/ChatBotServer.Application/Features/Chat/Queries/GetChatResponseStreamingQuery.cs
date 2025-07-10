using MediatR;

namespace ChatBotServer.Application.Features.Chat.Queries;

public class GetChatResponseStreamingQuery : IRequest<IAsyncEnumerable<string>>
{
    public string Content { get; set; } = string.Empty;
    public int? ConversationId { get; set; }
}