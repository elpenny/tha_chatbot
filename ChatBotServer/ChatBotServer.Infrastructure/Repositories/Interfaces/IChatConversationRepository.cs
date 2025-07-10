using ChatBotServer.Domain.Entities;

namespace ChatBotServer.Infrastructure.Repositories.Interfaces
{
    public interface IChatConversationRepository : IRepository<ChatConversation>
    {
        Task<ChatConversation?> GetConversationWithMessagesAsync(int conversationId);
        Task<IEnumerable<ChatConversation>> GetUserConversationsAsync(string userId);
    }
}
