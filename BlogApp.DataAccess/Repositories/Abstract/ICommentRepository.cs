using BlogApp.Entities;

namespace BlogApp.DataAccess.Repositories.Abstract
{
    public interface ICommentRepository : IRepository<Comment>
    {
        Task<IEnumerable<Comment>> GetAllWithDetailsAsync();
        Task<Comment?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<Comment>> GetByBlogPostIdAsync(int blogPostId);
        Task<IEnumerable<Comment>> GetByUserIdAsync(int userId);
        Task<bool> IsOwnedByUserAsync(int commentId, int userId);
    }
} 