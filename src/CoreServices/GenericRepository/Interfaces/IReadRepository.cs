using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CoreServices.GenericRepository.Interfaces
{
    public interface IReadRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAll();
        Task<T?> GetById(int id);
        Task<T?> GetByEmail(Expression<Func<T, bool>> email);
        Task<(IEnumerable<T>, int)> FilterAsync(
            Expression<Func<T, bool>> predicate,
            int page,
            int pageSize);
    }
}