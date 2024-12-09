using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CoreServices.GenericRepository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CoreServices.GenericRepository
{
    public class ReadRepository<T, TContext> : IReadRepository<T>
        where T : class
        where TContext : DbContext
    {

        private readonly TContext _context;
        private readonly DbSet<T> _dbSet;

        public ReadRepository(TContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T?> GetById(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<T?> GetByEmail(Expression<Func<T, bool>> email)
        {
            return await _dbSet.FirstOrDefaultAsync(email);
        }

        public async Task<(IEnumerable<T>, int)> FilterAsync(
            Expression<Func<T, bool>> predicate,
            int page,
            int pageSize)
        {
            var query = _dbSet.Where(predicate);

            var totalCount = await query.CountAsync();

            var paginatedData = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (paginatedData, totalCount);
        }

    }
}