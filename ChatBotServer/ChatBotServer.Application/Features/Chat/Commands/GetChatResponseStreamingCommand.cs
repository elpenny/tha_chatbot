using ChatBotServer.Application.Features.Chat.Results;
using MediatR;

namespace ChatBotServer.Application.Features.Chat.Commands;

public class GetChatResponseStreamingCommand : IRequest<StreamingChatResult>
{
    public string Content { get; set; } = string.Empty;
    public int? ConversationId { get; set; }
}