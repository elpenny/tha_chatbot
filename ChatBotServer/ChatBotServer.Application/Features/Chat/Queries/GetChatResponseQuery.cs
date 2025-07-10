using MediatR;

namespace ChatBotServer.Application.Features.Chat.Queries;

public class GetChatResponseQuery : IRequest<GetChatResponseResult>
{
    public string Content { get; set; } = string.Empty;
    public int? ConversationId { get; set; }
}

public class GetChatResponseResult
{
    public string Content { get; set; } = string.Empty;
    public bool IsComplete { get; set; }
}