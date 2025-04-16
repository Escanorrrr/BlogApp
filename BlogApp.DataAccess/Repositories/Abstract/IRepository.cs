using System.Linq.Expressions;

namespace BlogApp.DataAccess.Repositories.Abstract
{
    public interface IRepository<T> where T : class
    {
        // Temel CRUD operasyonları
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        
        // İlişkili verileri yüklemek için
        Task<T?> GetByIdWithIncludesAsync(int id, params Expression<Func<T, object>>[] includeProperties);
        Task<IEnumerable<T>> GetAllWithIncludesAsync(params Expression<Func<T, object>>[] includeProperties);
        
        // Toplu işlemler
        Task AddRangeAsync(IEnumerable<T> entities);
        Task DeleteRangeAsync(IEnumerable<T> entities);
        
        // Sorgu işlemleri
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
        Task<int> CountAsync(Expression<Func<T, bool>> predicate);
    }
} 