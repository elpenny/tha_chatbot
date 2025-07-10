using ChatBotServer.Domain.Entities;
using ChatBotServer.Infrastructure.Data;
using ChatBotServer.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChatBotServer.Infrastructure.Repositories
{
    public class ChatMessageRepository : Repository<ChatMessage>, IChatMessageRepository
    {
        public ChatMessageRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ChatMessage>> GetMessagesByConversationIdAsync(int conversationId)
        {
            return await _dbSet
                .Where(m => m.ConversationId == conversationId)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();
        }
    }
}
