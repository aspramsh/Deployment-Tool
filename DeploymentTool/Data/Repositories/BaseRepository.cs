using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T>
        where T : class, new()
    {
        /// <summary>
        /// Context property
        /// </summary>
        private readonly ProjectPublisherContext _context;

        public IQueryable<T> Entities => _context.Set<T>();

        /// <summary>
        /// Constructor with db context parameter
        /// </summary>
        /// <param name="context"></param>
        public BaseRepository(ProjectPublisherContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all entities
        /// </summary>
        /// <returns></returns>
        public virtual async Task<List<T>> GetAllAsync()
        {
            return await Entities.ToListAsync();
        }

        /// <summary>
        /// Get count of records
        /// </summary>
        /// <returns></returns>
        public virtual async Task<int> CountAsync()
        {
            return await Entities.CountAsync();
        }

        /// <summary>
        /// Filter with multiple lambda expressions or not 
        /// </summary>
        /// <param name="includeProperties"></param>
        /// <returns></returns>
        public virtual IQueryable<T> GetAllIncluding(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = Entities;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        /// <summary>
        /// Get single record for which the predicate is true
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<T> GetSingleAsync(Expression<Func<T, bool>> predicate)
        {
            return await Entities.FirstOrDefaultAsync(predicate);
        }

        /// <summary>
        /// Get single record which is first filtered and for which predicate is true
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="includeProperties"></param>
        /// <returns></returns>
        public async Task<T> GetSingleAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = Entities;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return await query.Where(predicate).FirstOrDefaultAsync();
        }


        /// <summary>
        /// Get Records for which the predicate is true
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<T>> FindByAsync(Expression<Func<T, bool>> predicate)
        {
            return await Entities.Where(predicate).ToListAsync();
        }

        public virtual T Add(T entity)
        {
            _context.Entry(entity);
            return _context.Set<T>().Add(entity).Entity;
        }

        public virtual void Update(T entity)
        {
            EntityEntry dbEntityEntry = _context.Entry(entity);
            dbEntityEntry.State = EntityState.Modified;
        }

        public virtual void Delete(T entity)
        {
            EntityEntry dbEntityEntry = _context.Entry(entity);
            dbEntityEntry.State = EntityState.Deleted;
        }

        /// <summary>
        /// Delete all record for which 
        /// </summary>
        /// <param name="predicate"></param>
        public virtual void DeleteWhere(Expression<Func<T, bool>> predicate)
        {
            IEnumerable<T> entities = Entities.Where(predicate);

            foreach (var entity in entities)
            {
                _context.Entry(entity).State = EntityState.Deleted;
            }
        }

        public virtual async Task<bool> SaveAsync()
        {
            int rows = await _context.SaveChangesAsync();
            return rows != 0;
        }
    }
}
