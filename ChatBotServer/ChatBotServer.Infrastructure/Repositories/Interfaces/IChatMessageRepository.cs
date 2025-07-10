using ChatBotServer.Domain.Entities;

namespace ChatBotServer.Infrastructure.Repositories.Interfaces;

public interface IChatMessageRepository : IRepository<ChatMessage>
{
    Task<IEnumerable<ChatMessage>> GetMessagesByConversationIdAsync(int conversationId);
    Task<ChatMessage?> GetMessageWithConversationAsync(int messageId);
}