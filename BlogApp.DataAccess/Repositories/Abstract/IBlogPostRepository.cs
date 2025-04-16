using BlogApp.Entities;

namespace BlogApp.DataAccess.Repositories.Abstract
{
    public interface IBlogPostRepository : IRepository<BlogPost>
    {
        Task<IEnumerable<BlogPost>> GetAllWithDetailsAsync();
        Task<BlogPost?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<BlogPost>> GetByCategoryIdAsync(int categoryId);
        Task<IEnumerable<BlogPost>> GetByUserIdAsync(int userId);
        Task<bool> IsOwnedByUserAsync(int blogPostId, int userId);
    }
} 