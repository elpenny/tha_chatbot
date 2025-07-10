using MediatR;

namespace ChatBotServer.Application.Features.Chat.Commands;

public class UpdateMessageRatingCommand : IRequest<bool>
{
    public int MessageId { get; set; }
    public int? Rating { get; set; } // 1 for thumbs up, -1 for thumbs down, null for no rating
}