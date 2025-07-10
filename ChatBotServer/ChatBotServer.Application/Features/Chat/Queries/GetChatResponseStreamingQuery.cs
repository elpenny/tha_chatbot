using ChatBotServer.Application.Features.Chat.Results;
using MediatR;

namespace ChatBotServer.Application.Features.Chat.Queries;

public class GetChatResponseStreamingQuery : IRequest<StreamingChatResult>
{
    public string Content { get; set; } = string.Empty;
    public int? ConversationId { get; set; }
}