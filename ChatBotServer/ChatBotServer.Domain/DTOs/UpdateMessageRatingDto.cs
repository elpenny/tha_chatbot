namespace ChatBotServer.Domain.DTOs;

public class UpdateMessageRatingDto
{
    public int MessageId { get; set; }
    public int? Rating { get; set; } // 1 for thumbs up, -1 for thumbs down, null to remove rating
}