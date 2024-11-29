using Microsoft.EntityFrameworkCore;
using SchoolAPI.Business.Data;

namespace CoreServices.GenericRepository
{
    public class Repository<T, TContext> : IRepository<T> 
        where T : class 
        where TContext : DbContext
    {
        private readonly TContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(TContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<T> Add(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> Delete(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null) return false;
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T?> GetById(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<T> Update(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}