using ChatBotServer.Domain.Entities;
using ChatBotServer.Infrastructure.Data;
using ChatBotServer.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChatBotServer.Infrastructure.Repositories;

public class ChatMessageRepository(ApplicationDbContext context)
    : Repository<ChatMessage>(context), IChatMessageRepository
{
    public async Task<IEnumerable<ChatMessage>> GetMessagesByConversationIdAsync(int conversationId)
    {
        return await DbSet
            .Where(m => m.ConversationId == conversationId)
            .OrderBy(m => m.CreatedAt)
            .ToListAsync();
    }

    public async Task<ChatMessage?> GetMessageWithConversationAsync(int messageId)
    {
        return await DbSet
            .Include(m => m.Conversation)
            .FirstOrDefaultAsync(m => m.Id == messageId);
    }
}