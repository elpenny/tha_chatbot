using ChatBotServer.Application.Features.Chat.Queries;
using ChatBotServer.Domain.Interfaces;
using MediatR;

namespace ChatBotServer.Application.Features.Chat.Handlers;

public class GetChatResponseStreamingHandler : IRequestHandler<GetChatResponseStreamingQuery, IAsyncEnumerable<string>>
{
    private readonly IChatBotService _chatBotService;

    public GetChatResponseStreamingHandler(IChatBotService chatBotService)
    {
        _chatBotService = chatBotService;
    }

    public Task<IAsyncEnumerable<string>> Handle(GetChatResponseStreamingQuery request, CancellationToken cancellationToken)
    {
        var streamingResponse = _chatBotService.GenerateStreamingResponseAsync(request.Content, cancellationToken);
        return Task.FromResult(streamingResponse);
    }
}