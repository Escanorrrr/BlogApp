using BlogApp.DataAccess.Context;
using BlogApp.DataAccess.Repositories.Abstract;
using BlogApp.Entities;

namespace BlogApp.DataAccess.Repositories.Concrete
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(BlogDbContext context) : base(context)
        {
        }
        
        // Eğer kategoriye özel ekstra metodlar gerekirse buraya eklenebilir
    }
} 