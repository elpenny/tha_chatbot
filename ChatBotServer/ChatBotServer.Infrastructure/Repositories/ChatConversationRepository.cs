using ChatBotServer.Domain.Entities;
using ChatBotServer.Infrastructure.Data;
using ChatBotServer.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChatBotServer.Infrastructure.Repositories;

public class ChatConversationRepository(ApplicationDbContext context)
    : Repository<ChatConversation>(context), IChatConversationRepository
{
    public async Task<ChatConversation?> GetConversationWithMessagesAsync(int conversationId)
    {
        return await DbSet
            .Include(c => c.Messages.OrderBy(m => m.CreatedAt))
            .FirstOrDefaultAsync(c => c.Id == conversationId);
    }

    public async Task<IEnumerable<ChatConversation>> GetRecentConversationsAsync(int limit = 10)
    {
        return await DbSet
            .OrderByDescending(c => c.UpdatedAt ?? c.CreatedAt)
            .Take(limit)
            .ToListAsync();
    }
}