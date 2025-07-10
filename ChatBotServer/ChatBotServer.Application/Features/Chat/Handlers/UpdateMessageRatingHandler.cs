using ChatBotServer.Application.Features.Chat.Commands;
using ChatBotServer.Infrastructure.Repositories.Interfaces;
using MediatR;

namespace ChatBotServer.Application.Features.Chat.Handlers;

public class UpdateMessageRatingHandler : IRequestHandler<UpdateMessageRatingCommand, bool>
{
    private readonly IChatMessageRepository _messageRepository;

    public UpdateMessageRatingHandler(IChatMessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }

    public async Task<bool> Handle(UpdateMessageRatingCommand request, CancellationToken cancellationToken)
    {
        var message = await _messageRepository.GetByIdAsync(request.MessageId);
        
        if (message == null)
            return false;

        message.Rating = request.Rating;
        await _messageRepository.UpdateAsync(message);
        await _messageRepository.SaveChangesAsync();

        return true;
    }
}