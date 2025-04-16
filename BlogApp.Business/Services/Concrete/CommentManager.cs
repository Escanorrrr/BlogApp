using BlogApp.Business.Services.Abstract;
using BlogApp.DataAccess.Repositories.Abstract;
using BlogApp.Entities;
using BlogApp.Entities.DTOs;
using BlogApp.Entities.Helpers;
using Microsoft.AspNetCore.Http;
using BlogApp.Business.Helpers;

namespace BlogApp.Business.Services.Concrete
{
    public class CommentManager : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CommentManager(ICommentRepository commentRepository, IHttpContextAccessor httpContextAccessor)
        {
            _commentRepository = commentRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        private CommentDto MapToDto(Comment comment)
        {
            if (comment == null)
                throw new ArgumentNullException(nameof(comment));

            return new CommentDto
            {
                Id = comment.Id,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                UserName = comment.User?.Username ?? "Anonim",
                BlogPostId = comment.BlogPostId,
                UserId = comment.UserId
            };
        }

        public async Task<Result<CommentDto>> GetByIdAsync(int id)
        {
            var comment = await _commentRepository.GetByIdWithDetailsAsync(id);

            if (comment == null)
                return Result<CommentDto>.FailureResult("Yorum bulunamadı");

            var commentDto = MapToDto(comment);
            return Result<CommentDto>.SuccessResult(commentDto);
        }

        public async Task<Result<List<CommentDto>>> GetAllAsync()
        {
            var comments = await _commentRepository.GetAllWithDetailsAsync();

            var commentDtos = comments.Select(MapToDto).ToList();
            return Result<List<CommentDto>>.SuccessResult(commentDtos);
        }

        public async Task<Result<List<CommentDto>>> GetByBlogPostIdAsync(int blogPostId)
        {
            var comments = await _commentRepository.GetByBlogPostIdAsync(blogPostId);

            var commentDtos = comments.Select(MapToDto).ToList();
            return Result<List<CommentDto>>.SuccessResult(commentDtos);
        }

        public async Task<Result<CommentDto>> AddAsync(CommentCreateDto commentCreateDto, int userId)
        {
            var comment = new Comment
            {
                Content = commentCreateDto.Content,
                BlogPostId = commentCreateDto.BlogPostId,
                UserId = userId,
                CreatedAt = DateTime.Now
            };

            var addedComment = await _commentRepository.AddAsync(comment);
            var commentWithDetails = await _commentRepository.GetByIdWithDetailsAsync(addedComment.Id);

            if (commentWithDetails == null)
                return Result<CommentDto>.FailureResult("Yorum eklenirken bir hata oluştu");

            var commentDto = MapToDto(commentWithDetails);
            return Result<CommentDto>.SuccessResult(commentDto, "Yorum başarıyla eklendi");
        }

        public async Task<Result<CommentDto>> UpdateAsync(CommentUpdateDto commentUpdateDto, int userId)
        {
            var comment = await _commentRepository.GetByIdWithDetailsAsync(commentUpdateDto.Id);

            if (comment == null)
                return Result<CommentDto>.FailureResult("Yorum bulunamadı");

            bool isAdmin = UserHelper.IsLoggedInUserAdmin(_httpContextAccessor.HttpContext);
            
            if (!isAdmin && comment.UserId != userId)
                return Result<CommentDto>.FailureResult("Bu yorumu güncelleme yetkiniz yok");

            comment.Content = commentUpdateDto.Content;
            var updatedComment = await _commentRepository.UpdateAsync(comment);

            var commentDto = MapToDto(updatedComment);
            return Result<CommentDto>.SuccessResult(commentDto, "Yorum başarıyla güncellendi");
        }

        public async Task<Result<object>> DeleteAsync(int id, int userId)
        {
            var comment = await _commentRepository.GetByIdAsync(id);
            if (comment == null)
                return Result<object>.FailureResult("Yorum bulunamadı");

            bool isAdmin = UserHelper.IsLoggedInUserAdmin(_httpContextAccessor.HttpContext);
            
            if (!isAdmin && comment.UserId != userId)
                return Result<object>.FailureResult("Bu yorumu silme yetkiniz yok");

            await _commentRepository.DeleteAsync(comment);
            return Result<object>.SuccessResult(null, "Yorum başarıyla silindi");
        }
    }
} 