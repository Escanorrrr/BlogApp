using BlogApp.DataAccess.Context;
using BlogApp.DataAccess.Repositories.Abstract;
using BlogApp.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.DataAccess.Repositories.Concrete
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(BlogDbContext context) : base(context)
        {
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<bool> IsEmailUniqueAsync(string email)
        {
            return !await _dbSet.AnyAsync(u => u.Email == email);
        }

        public async Task<bool> IsUsernameUniqueAsync(string username)
        {
            return !await _dbSet.AnyAsync(u => u.Username == username);
        }

        public async Task<User?> GetByEmailWithDetailsAsync(string email)
        {
            return await _dbSet
                .Include(u => u.BlogPosts)
                .Include(u => u.Comments)
                .FirstOrDefaultAsync(u => u.Email == email);
        }
    }
} 