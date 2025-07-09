using ChatBotServer.Infrastructure.Data;
using ChatBotServer.Infrastructure.Data.Models;
using ChatBotServer.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChatBotServer.Infrastructure.Repositories
{
    public class ChatConversationRepository : Repository<ChatConversation>, IChatConversationRepository
    {
        public ChatConversationRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<ChatConversation?> GetConversationWithMessagesAsync(int conversationId)
        {
            return await _dbSet
                .Include(c => c.Messages.OrderBy(m => m.CreatedAt))
                .FirstOrDefaultAsync(c => c.Id == conversationId);
        }

        public async Task<IEnumerable<ChatConversation>> GetUserConversationsAsync(string userId)
        {
            return await _dbSet
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.UpdatedAt ?? c.CreatedAt)
                .ToListAsync();
        }
    }
}
