using BlogApp.DataAccess.Context;
using BlogApp.DataAccess.Repositories.Abstract;
using BlogApp.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.DataAccess.Repositories.Concrete
{
    public class BlogPostRepository : Repository<BlogPost>, IBlogPostRepository
    {
        public BlogPostRepository(BlogDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<BlogPost>> GetAllWithDetailsAsync()
        {
            Console.WriteLine("Executing GetAllWithDetailsAsync in BlogPostRepository"); // Debug log
            var result = await _dbSet
                .Include(b => b.Category)
                .Include(b => b.User)
                .ToListAsync();
            Console.WriteLine($"Found {result.Count} posts in database"); // Debug log
            return result;
        }

        public async Task<BlogPost?> GetByIdWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(b => b.Category)
                .Include(b => b.User)
                .Include(b => b.Comments)
                    .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<IEnumerable<BlogPost>> GetByCategoryIdAsync(int categoryId)
        {
            return await _dbSet
                .Include(b => b.Category)
                .Include(b => b.User)
                .Where(b => b.CategoryId == categoryId)
                .ToListAsync();
        }

        public async Task<IEnumerable<BlogPost>> GetByUserIdAsync(int userId)
        {
            return await _dbSet
                .Include(b => b.Category)
                .Where(b => b.UserId == userId)
                .ToListAsync();
        }

        public async Task<bool> IsOwnedByUserAsync(int blogPostId, int userId)
        {
            return await _dbSet.AnyAsync(b => b.Id == blogPostId && b.UserId == userId);
        }
    }
} 