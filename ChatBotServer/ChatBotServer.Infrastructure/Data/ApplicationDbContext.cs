using ChatBotServer.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatBotServer.Infrastructure.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<ChatConversation> ChatConversations { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure ChatConversation
            modelBuilder.Entity<ChatConversation>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).HasMaxLength(200);
                entity.Property(e => e.CreatedAt).IsRequired();
                
                // One-to-many relationship
                entity.HasMany(e => e.Messages)
                      .WithOne(e => e.Conversation)
                      .HasForeignKey(e => e.ConversationId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure ChatMessage
            modelBuilder.Entity<ChatMessage>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Content).IsRequired();
                entity.Property(e => e.Role).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.Rating).IsRequired(false);
                entity.Property(e => e.ConversationId).IsRequired();
            });
        }
    }
}
