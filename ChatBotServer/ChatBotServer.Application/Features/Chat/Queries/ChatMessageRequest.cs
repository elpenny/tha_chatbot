using System.ComponentModel.DataAnnotations;

namespace ChatBotServer.Application.Features.Chat.Queries;

/// <summary>
/// Request model for sending a chat message
/// </summary>
public class ChatMessageRequest
{
    /// <summary>
    /// The content of the message to send to the chatbot
    /// </summary>
    [Required]
    [StringLength(4000, MinimumLength = 1, ErrorMessage = "Message content must be between 1 and 4000 characters")]
    public string Content { get; set; } = string.Empty;
    
    /// <summary>
    /// Optional conversation ID. If null, a new conversation will be created
    /// </summary>
    public int? ConversationId { get; set; }
}