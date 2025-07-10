using ChatBotServer.Application.Features.Chat.Queries;
using ChatBotServer.Infrastructure.Repositories.Interfaces;
using MediatR;

namespace ChatBotServer.Application.Features.Chat.Handlers;

public class GetConversationHistoryHandler : IRequestHandler<GetConversationHistoryQuery, GetConversationHistoryResult>
{
    private readonly IChatConversationRepository _conversationRepository;

    public GetConversationHistoryHandler(IChatConversationRepository conversationRepository)
    {
        _conversationRepository = conversationRepository;
    }

    public async Task<GetConversationHistoryResult> Handle(GetConversationHistoryQuery request, CancellationToken cancellationToken)
    {
        var conversation = await _conversationRepository.GetConversationWithMessagesAsync(request.ConversationId);
        
        if (conversation == null)
            throw new InvalidOperationException($"Conversation with ID {request.ConversationId} not found");

        return new GetConversationHistoryResult
        {
            Id = conversation.Id,
            Title = conversation.Title,
            CreatedAt = conversation.CreatedAt,
            UpdatedAt = conversation.UpdatedAt,
            Messages = conversation.Messages.Select(m => new ChatMessageResult
            {
                Id = m.Id,
                Content = m.Content,
                Role = m.Role,
                CreatedAt = m.CreatedAt,
                Rating = m.Rating
            }).ToList()
        };
    }
}