using BlogApp.Entities;
using BlogApp.Entities.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlogApp.Business.Services.Abstract
{
    public interface ICategoryService
    {
        Task<Result<List<Category>>> GetAllAsync();
        Task<Result<Category>> GetByIdAsync(int id);
        Task<Result<Category>> AddAsync(Category category);
        Task<Result<Category>> UpdateAsync(Category category);
        Task<Result<object>> DeleteAsync(int id);
    }
} 