using BlogApp.DataAccess.Repositories.Concrete;
using BlogApp.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlogApp.DataAccess.Repositories.Abstract
{
    public interface ICategoryRepository : IRepository<Category>
    {
        // Eğer kategoriye özel ekstra metodlar gerekirse buraya eklenebilir
    }
} 