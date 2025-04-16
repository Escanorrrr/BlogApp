using BlogApp.DataAccess.Context;
using BlogApp.DataAccess.Repositories.Abstract;
using BlogApp.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.DataAccess.Repositories.Concrete
{
    public class CommentRepository : Repository<Comment>, ICommentRepository
    {
        public CommentRepository(BlogDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Comment>> GetAllWithDetailsAsync()
        {
            return await _dbSet
                .Include(c => c.User)
                .Include(c => c.BlogPost)
                .ToListAsync();
        }

        public async Task<Comment?> GetByIdWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(c => c.User)
                .Include(c => c.BlogPost)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Comment>> GetByBlogPostIdAsync(int blogPostId)
        {
            return await _dbSet
                .Include(c => c.User)
                .Where(c => c.BlogPostId == blogPostId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Comment>> GetByUserIdAsync(int userId)
        {
            return await _dbSet
                .Include(c => c.BlogPost)
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }

        public async Task<bool> IsOwnedByUserAsync(int commentId, int userId)
        {
            return await _dbSet.AnyAsync(c => c.Id == commentId && c.UserId == userId);
        }
    }
} 