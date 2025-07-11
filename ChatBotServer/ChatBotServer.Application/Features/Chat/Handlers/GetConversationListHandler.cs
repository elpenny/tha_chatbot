using ChatBotServer.Application.Features.Chat.Queries;
using ChatBotServer.Application.Features.Chat.Results;
using ChatBotServer.Infrastructure.Repositories.Interfaces;
using MediatR;

namespace ChatBotServer.Application.Features.Chat.Handlers;

public class GetConversationListHandler(IChatConversationRepository conversationRepository)
    : IRequestHandler<GetConversationListQuery, GetConversationListResult>
{
    public async Task<GetConversationListResult> Handle(GetConversationListQuery request, CancellationToken cancellationToken)
    {
        var conversations = await conversationRepository.GetRecentConversationsAsync(request.Limit);
        
        var conversationSummaries = conversations.Select(c => new ConversationSummary
        {
            Id = c.Id,
            Title = c.Title,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt,
            MessageCount = c.Messages?.Count ?? 0,
            LastMessage = c.Messages?.LastOrDefault()?.Content
        });

        return new GetConversationListResult
        {
            Conversations = conversationSummaries
        };
    }
}
