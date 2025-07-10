using ChatBotServer.Application.Features.Chat.Queries;
using ChatBotServer.Domain.Interfaces;
using MediatR;

namespace ChatBotServer.Application.Features.Chat.Handlers;

public class GetChatResponseHandler : IRequestHandler<GetChatResponseQuery, GetChatResponseResult>
{
    private readonly IChatBotService _chatBotService;

    public GetChatResponseHandler(IChatBotService chatBotService)
    {
        _chatBotService = chatBotService;
    }

    public async Task<GetChatResponseResult> Handle(GetChatResponseQuery request, CancellationToken cancellationToken)
    {
        var response = await _chatBotService.GenerateResponseAsync(request.Content, cancellationToken);

        return new GetChatResponseResult
        {
            Content = response,
            IsComplete = true
        };
    }
}