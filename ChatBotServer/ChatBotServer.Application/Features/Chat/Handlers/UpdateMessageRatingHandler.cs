using ChatBotServer.Application.Features.Chat.Commands;
using ChatBotServer.Infrastructure.Repositories.Interfaces;
using MediatR;

namespace ChatBotServer.Application.Features.Chat.Handlers;

public class UpdateMessageRatingHandler(IChatMessageRepository messageRepository)
    : IRequestHandler<UpdateMessageRatingCommand, bool>
{
    public async Task<bool> Handle(UpdateMessageRatingCommand request, CancellationToken cancellationToken)
    {
        var message = await messageRepository.GetByIdAsync(request.MessageId);
        
        if (message == null)
            return false;

        message.Rating = request.Rating;
        await messageRepository.UpdateAsync(message);
        await messageRepository.SaveChangesAsync();

        return true;
    }
}