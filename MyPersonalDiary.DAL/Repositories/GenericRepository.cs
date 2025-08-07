using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MyPersonalDiary.DAL.ApplicationDbContext;
using MyPersonalDiary.DAL.InterfacesRepositories;

namespace MyPersonalDiary.DAL.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class, new()
    {
        internal AppDbContext _context;
        internal DbSet<TEntity> _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public virtual async Task<IEnumerable<TEntity>> GetAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "")
        {
            IQueryable<TEntity> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            if (orderBy != null)
            {
                return await orderBy(query).ToListAsync();
            }

            return await query.ToListAsync();
        }

        public virtual async Task<TEntity> GetByIDAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task InsertAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public virtual async Task DeleteAsync(object id)
        {
            TEntity entityToDelete = await _dbSet.FindAsync(id);
            if (entityToDelete != null)
            {
                await DeleteAsync(entityToDelete);
            }
        }

        public virtual async Task DeleteAsync(TEntity entityToDelete)
        {
            if (_context.Entry(entityToDelete).State == EntityState.Detached)
            {
                _dbSet.Attach(entityToDelete);
            }
            _dbSet.Remove(entityToDelete);
            await Task.CompletedTask;
        }

        public virtual async Task UpdateAsync(TEntity entityToUpdate)
        {
            _dbSet.Attach(entityToUpdate);
            _context.Entry(entityToUpdate).State = EntityState.Modified;
            await Task.CompletedTask;
        }
    }
}
