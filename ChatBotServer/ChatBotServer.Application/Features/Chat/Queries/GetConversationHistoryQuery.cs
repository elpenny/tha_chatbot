using ChatBotServer.Domain.Entities;
using MediatR;

namespace ChatBotServer.Application.Features.Chat.Queries;

public class GetConversationHistoryQuery : IRequest<GetConversationHistoryResult>
{
    public int ConversationId { get; set; }
}

public class GetConversationHistoryResult
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<ChatMessageResult> Messages { get; set; } = new();
}

public class ChatMessageResult
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public MessageRole Role { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? Rating { get; set; }
}