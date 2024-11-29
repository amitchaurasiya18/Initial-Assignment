
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreServices.GenericRepository
{
    public interface IRepository<T> where T : class
    {
        Task<T> Add(T entity);
        Task<bool> Delete(int id);
        Task<IEnumerable<T>> GetAll();
        Task<T?> GetById(int id);
        Task<T> Update(T entity);
    }
}