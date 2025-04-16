using System.Linq.Expressions;
using BlogApp.DataAccess.Context;
using BlogApp.DataAccess.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.DataAccess.Repositories.Concrete
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly BlogDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(BlogDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task<T> UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public virtual async Task<T?> GetByIdWithIncludesAsync(int id, params Expression<Func<T, object>>[] includeProperties)
        {
            var query = includeProperties.Aggregate(_dbSet.AsQueryable(),
                (current, includeProperty) => current.Include(includeProperty));

            // Bu kısım biraz tricky, çünkü generic T için Id property'sini bulmamız gerekiyor
            // Basit bir çözüm olarak dynamic kullanıyoruz
            return await query.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
        }

        public virtual async Task<IEnumerable<T>> GetAllWithIncludesAsync(params Expression<Func<T, object>>[] includeProperties)
        {
            var query = includeProperties.Aggregate(_dbSet.AsQueryable(),
                (current, includeProperty) => current.Include(includeProperty));

            return await query.ToListAsync();
        }

        public virtual async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
        }

        public virtual async Task DeleteRangeAsync(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
            await _context.SaveChangesAsync();
        }

        public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }

        public virtual async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.CountAsync(predicate);
        }
    }
} 