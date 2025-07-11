using ChatBotServer.Application.Features.Chat.Results;
using MediatR;

namespace ChatBotServer.Application.Features.Chat.Queries;

public class GetConversationListQuery : IRequest<GetConversationListResult>
{
    public int Limit { get; set; } = 10;
}