using BlogApp.Entities;
using BlogApp.Entities.Dtos;
using BlogApp.Entities.Helpers;

namespace BlogApp.Business.Services.Abstract
{
    public interface IBlogPostService
    {
        Task<Result<List<BlogPostDto>>> GetAllAsync();
        Task<Result<BlogPostDto>> GetByIdAsync(int id);
        Task<Result<BlogPost>> GetByIdWithOwnerAsync(int id);
        Task<Result<BlogPost>> AddAsync(BlogPostAddDto dto, int userId);
        Task<Result<BlogPost>> UpdateAsync(BlogPostUpdateDto dto, int userId, bool isAdmin);
        Task<Result<object>> DeleteAsync(int id, int userId, bool isAdmin);
        Task<Result<List<BlogPostDto>>> GetByCategoryIdAsync(int categoryId);
        Task<Result<List<BlogPostDto>>> GetRelatedPostsAsync(int id);
        Task<Result<List<BlogPostDto>>> GetByUserIdAsync(int userId);
    }
}

