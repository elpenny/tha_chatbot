using ChatBotServer.Domain.Entities;
using MediatR;

namespace ChatBotServer.Application.Features.Chat.Queries;

/// <summary>
/// Query to retrieve conversation history
/// </summary>
public class GetConversationHistoryQuery : IRequest<GetConversationHistoryResult>
{
    /// <summary>
    /// The conversation ID to retrieve
    /// </summary>
    public int ConversationId { get; set; }
}

/// <summary>
/// Result containing conversation details and all messages
/// </summary>
public class GetConversationHistoryResult
{
    /// <summary>
    /// Conversation ID
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Conversation title
    /// </summary>
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// When the conversation was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// When the conversation was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
    
    /// <summary>
    /// All messages in the conversation
    /// </summary>
    public List<ChatMessageResult> Messages { get; set; } = new();
}

/// <summary>
/// Individual message within a conversation
/// </summary>
public class ChatMessageResult
{
    /// <summary>
    /// Message ID
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Message content
    /// </summary>
    public string Content { get; set; } = string.Empty;
    
    /// <summary>
    /// Message role (User or Assistant)
    /// </summary>
    public MessageRole Role { get; set; }
    
    /// <summary>
    /// When the message was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Message rating (1 for thumbs up, -1 for thumbs down, null for no rating)
    /// </summary>
    public int? Rating { get; set; }
}