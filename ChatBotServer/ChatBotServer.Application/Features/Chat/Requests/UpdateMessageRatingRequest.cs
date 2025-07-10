using System.ComponentModel.DataAnnotations;

namespace ChatBotServer.Application.Features.Chat.Requests;

/// <summary>
/// Request model for updating message rating
/// </summary>
public class UpdateMessageRatingRequest
{
    /// <summary>
    /// Rating value: 1 for thumbs up, -1 for thumbs down, null to remove rating
    /// </summary>
    [Range(-1, 1, ErrorMessage = "Rating must be -1, 0, or 1")]
    public int? Rating { get; set; }
}