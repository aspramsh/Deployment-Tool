using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public interface IBaseRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync();

        Task<int> CountAsync();

        IQueryable<T> GetAllIncluding(params Expression<Func<T, object>>[] includeProperties);

        Task<T> GetSingleAsync(Expression<Func<T, bool>> predicate);

        Task<T> GetSingleAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);

        Task<IEnumerable<T>> FindByAsync(Expression<Func<T, bool>> predicate);

        T Add(T entity);

        void Update(T entity);

        void Delete(T entity);

        void DeleteWhere(Expression<Func<T, bool>> predicate);

        Task<bool> SaveAsync();
    }
}
