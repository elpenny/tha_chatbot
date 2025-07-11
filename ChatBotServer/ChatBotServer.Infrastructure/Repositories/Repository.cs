using System.Linq.Expressions;
using ChatBotServer.Infrastructure.Data;
using ChatBotServer.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChatBotServer.Infrastructure.Repositories
{
    public class Repository<T>(ApplicationDbContext context) : IRepository<T>
        where T : class
    {
        protected readonly DbSet<T> DbSet = context.Set<T>();

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await DbSet.ToListAsync();
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await DbSet.Where(predicate).ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await DbSet.FindAsync(id);
        }

        public async Task<T> AddAsync(T entity)
        {
            await DbSet.AddAsync(entity);
            return entity;
        }

        public Task UpdateAsync(T entity)
        {
            DbSet.Attach(entity);
            context.Entry(entity).State = EntityState.Modified;
            return Task.CompletedTask;
        }

        public Task DeleteAsync(T entity)
        {
            DbSet.Remove(entity);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}
