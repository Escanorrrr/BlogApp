using BlogApp.Business.Services.Abstract;
using BlogApp.DataAccess.Repositories.Abstract;
using BlogApp.Entities;
using BlogApp.Entities.Dtos;
using BlogApp.Entities.Helpers;
using BlogApp.Entities.Enums;
using BlogApp.Entities.DTOs;
using HtmlAgilityPack;

namespace BlogApp.Business.Services.Concrete
{
    public class BlogPostManager : IBlogPostService
    {
        private readonly IBlogPostRepository _blogPostRepository;
        private readonly IFotoService _fotoService;

        public BlogPostManager(IBlogPostRepository blogPostRepository, IFotoService fotoService)
        {
            _blogPostRepository = blogPostRepository;
            _fotoService = fotoService;
        }

        private string StripHtml(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            
            var doc = new HtmlDocument();
            doc.LoadHtml(input);
            return doc.DocumentNode.InnerText;
        }

        public async Task<Result<List<BlogPostDto>>> GetAllAsync()
        {
            var posts = await _blogPostRepository.GetAllWithDetailsAsync();
            
            var postDtos = posts.Select(p => new BlogPostDto
            {
                Id = p.Id,
                Title = p.Title,
                Content = StripHtml(p.Content),
                ImagePath = p.ImagePath,
                PublishedDate = p.PublishedDate,
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.Name ?? "Kategorisiz",
                AuthorUsername = p.User?.Username ?? "Anonim",
                UserId = p.UserId
            }).ToList();

            return Result<List<BlogPostDto>>.SuccessResult(postDtos);
        }

        public async Task<Result<BlogPostDto>> GetByIdAsync(int id)
        {
            var post = await _blogPostRepository.GetByIdWithDetailsAsync(id);

            if (post == null)
                return Result<BlogPostDto>.FailureResult("Blog yazısı bulunamadı.");

            Console.WriteLine($"Debug - ImagePath: {post.ImagePath}"); // Debug log

            var dto = new BlogPostDto
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                ImagePath = post.ImagePath,
                PublishedDate = post.PublishedDate,
                CategoryName = post.Category?.Name ?? "Kategorisiz",
                CategoryId = post.CategoryId,
                AuthorUsername = post.User?.Username ?? "Anonim",
                UserId = post.UserId,
                Comments = post.Comments?.Select(c => new CommentDto
                {
                    Id = c.Id,
                    Content = StripHtml(c.Content),
                    CreatedAt = c.CreatedAt,
                    UserName = c.User?.Username ?? "Anonim",
                    BlogPostId = c.BlogPostId
                }).ToList() ?? new List<CommentDto>()
            };

            return Result<BlogPostDto>.SuccessResult(dto);
        }

        public async Task<Result<BlogPost>> AddAsync(BlogPostAddDto dto, int userId)
        {
            string? imagePath = null;
            
            if (!string.IsNullOrEmpty(dto.ImageBase64))
            {
                if (!_fotoService.IsBase64String(dto.ImageBase64))
                {
                    return Result<BlogPost>.FailureResult("Geçersiz fotoğraf formatı. Lütfen geçerli bir base64 formatında fotoğraf gönderin.");
                }
                
                try
                {
                    imagePath = _fotoService.Upload(dto.ImageBase64);
                    if (string.IsNullOrEmpty(imagePath))
                    {
                        return Result<BlogPost>.FailureResult("Fotoğraf yükleme hatası: Dosya kaydedilemedi.");
                    }
                }
                catch (Exception ex)
                {
                    return Result<BlogPost>.FailureResult($"Fotoğraf yükleme hatası: {ex.Message}");
                }
            }

            var post = new BlogPost
            {
                Title = dto.Title,
                Content = dto.Content,
                CategoryId = dto.CategoryId,
                ImagePath = imagePath,
                PublishedDate = DateTime.UtcNow,
                UserId = userId
            };

            var addedPost = await _blogPostRepository.AddAsync(post);
            return Result<BlogPost>.SuccessResult(addedPost, "Blog yazısı eklendi.");
        }

        public async Task<Result<BlogPost>> UpdateAsync(BlogPostUpdateDto dto, int userId, bool isAdmin)
        {
            if (string.IsNullOrWhiteSpace(dto.Title) || dto.Title.Length < 3)
                return Result<BlogPost>.FailureResult("Başlık en az 3 karakter olmalıdır.");

            if (string.IsNullOrWhiteSpace(dto.Content) || dto.Content.Length < 10)
                return Result<BlogPost>.FailureResult("İçerik en az 10 karakter olmalıdır.");

            var post = await _blogPostRepository.GetByIdAsync(dto.Id);
            if (post == null)
                return Result<BlogPost>.FailureResult("Blog yazısı bulunamadı.");

            if (!isAdmin && post.UserId != userId)
                return Result<BlogPost>.FailureResult("Bu blog yazısını güncelleme yetkiniz yok.");

            string? imagePath = post.ImagePath;
            
            if (!string.IsNullOrEmpty(dto.ImageBase64))
            {
                if (!_fotoService.IsBase64String(dto.ImageBase64))
                {
                    return Result<BlogPost>.FailureResult("Geçersiz fotoğraf formatı. Lütfen geçerli bir base64 formatında fotoğraf gönderin.");
                }
                
                try
                {
                    imagePath = _fotoService.Upload(dto.ImageBase64);
                    if (string.IsNullOrEmpty(imagePath))
                    {
                        return Result<BlogPost>.FailureResult("Fotoğraf yükleme hatası: Dosya kaydedilemedi.");
                    }
                }
                catch (Exception ex)
                {
                    return Result<BlogPost>.FailureResult($"Fotoğraf yükleme hatası: {ex.Message}");
                }
            }

            post.Title = dto.Title;
            post.Content = dto.Content;
            post.CategoryId = dto.CategoryId;
            post.ImagePath = imagePath;

            var updatedPost = await _blogPostRepository.UpdateAsync(post);
            return Result<BlogPost>.SuccessResult(updatedPost, "Blog yazısı başarıyla güncellendi.");
        }

        public async Task<Result<object>> DeleteAsync(int id, int userId, bool isAdmin)
        {
            var post = await _blogPostRepository.GetByIdAsync(id);
            if (post == null)
                return Result<object>.FailureResult("Blog yazısı bulunamadı.");
            
            if (!isAdmin && post.UserId != userId)
                return Result<object>.FailureResult("Bu blog yazısını silme yetkiniz yok.");

            await _blogPostRepository.DeleteAsync(post);
            return Result<object>.SuccessResult(null, "Blog yazısı silindi.");
        }

        public async Task<Result<BlogPost>> GetByIdWithOwnerAsync(int id)
        {
            var post = await _blogPostRepository.GetByIdWithDetailsAsync(id);
            if (post == null)
                return Result<BlogPost>.FailureResult("Blog yazısı bulunamadı.");

            post.Content = StripHtml(post.Content);

            return Result<BlogPost>.SuccessResult(post);
        }

        public async Task<Result<List<BlogPostDto>>> GetByCategoryIdAsync(int categoryId)
        {
            var posts = await _blogPostRepository.GetByCategoryIdAsync(categoryId);
            if (!posts.Any())
                return Result<List<BlogPostDto>>.FailureResult("Bu kategoride blog yazısı bulunamadı.");

            var postDtos = posts.Select(p => new BlogPostDto
            {
                Id = p.Id,
                Title = p.Title,
                Content = StripHtml(p.Content),
                ImagePath = p.ImagePath,
                PublishedDate = p.PublishedDate,
                CategoryName = p.Category?.Name ?? "Kategorisiz",
                AuthorUsername = p.User?.Username ?? "Anonim"
            }).ToList();

            return Result<List<BlogPostDto>>.SuccessResult(postDtos);
        }

        public async Task<Result<List<BlogPostDto>>> GetRelatedPostsAsync(int id)
        {
            try
            {
                var currentPost = await _blogPostRepository.GetByIdAsync(id);
                if (currentPost == null)
                    return Result<List<BlogPostDto>>.FailureResult("Blog yazısı bulunamadı.");

                var relatedPosts = await _blogPostRepository.GetAllAsync();
                var filteredPosts = relatedPosts
                    .Where(p => p.CategoryId == currentPost.CategoryId && p.Id != id)
                    .Take(5)
                    .Select(p => new BlogPostDto
                    {
                        Id = p.Id,
                        Title = p.Title,
                        Content = StripHtml(p.Content),
                        ImagePath = p.ImagePath,
                        PublishedDate = p.PublishedDate,
                        CategoryName = p.Category?.Name ?? "Kategorisiz",
                        CategoryId = p.CategoryId,
                        AuthorUsername = p.User?.Username ?? "Anonim",
                        UserId = p.UserId
                    })
                    .ToList();

                return Result<List<BlogPostDto>>.SuccessResult(filteredPosts);
            }
            catch (Exception ex)
            {
                return Result<List<BlogPostDto>>.FailureResult($"İlgili yazılar getirilirken bir hata oluştu: {ex.Message}");
            }
        }

        public async Task<Result<List<BlogPostDto>>> GetByUserIdAsync(int userId)
        {
            try
            {
                var posts = await _blogPostRepository.GetAllWithDetailsAsync();
                var userPosts = posts
                    .Where(p => p.UserId == userId)
                    .Select(p => new BlogPostDto
                    {
                        Id = p.Id,
                        Title = p.Title,
                        Content = StripHtml(p.Content),
                        ImagePath = p.ImagePath,
                        PublishedDate = p.PublishedDate,
                        CategoryId = p.CategoryId,
                        CategoryName = p.Category?.Name ?? "Kategorisiz",
                        AuthorUsername = p.User?.Username ?? "Anonim",
                        UserId = p.UserId
                    })
                    .ToList();

                return Result<List<BlogPostDto>>.SuccessResult(userPosts);
            }
            catch (Exception ex)
            {
                return Result<List<BlogPostDto>>.FailureResult($"Kullanıcının yazıları getirilirken bir hata oluştu: {ex.Message}");
            }
        }
    }
}

