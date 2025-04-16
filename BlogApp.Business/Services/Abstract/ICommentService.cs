using BlogApp.Entities;
using BlogApp.Entities.DTOs;
using BlogApp.Entities.Helpers;

namespace BlogApp.Business.Services.Abstract
{
    public interface ICommentService
    {
        Task<Result<CommentDto>> GetByIdAsync(int id);
        Task<Result<List<CommentDto>>> GetAllAsync();
        Task<Result<List<CommentDto>>> GetByBlogPostIdAsync(int blogPostId);
        Task<Result<CommentDto>> AddAsync(CommentCreateDto commentCreateDto, int userId);
        Task<Result<CommentDto>> UpdateAsync(CommentUpdateDto commentUpdateDto, int userId);
        Task<Result<object>> DeleteAsync(int id, int userId);
    }
} 