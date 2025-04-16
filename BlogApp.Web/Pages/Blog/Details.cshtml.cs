using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BlogApp.Entities.Dtos;
using BlogApp.Entities.DTOs;
using System.Text.Json;
using System.Security.Claims;
using BlogApp.Entities.Helpers;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;

namespace Web.Pages.Blog
{
    public class DetailsModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BlogPostDto BlogPost { get; set; }
        public List<BlogPostDto> RelatedPosts { get; set; } = new();
        public List<CommentDto> Comments { get; set; } = new();
        public bool IsAdmin { get; private set; }

        // Düzenle/Sil butonları için yetki kontrolü
        public bool CanEditPost => 
            User.FindFirst("IsAdmin")?.Value == "True" || // Kullanıcı admin mi?
            (BlogPost?.UserId == int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0")); // Veya post sahibi mi?

        [BindProperty]
        public CommentCreateDto NewComment { get; set; }

        public DetailsModel(IHttpClientFactory httpClientFactory, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClientFactory.CreateClient();
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _httpClient.BaseAddress = new Uri(configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5082");
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                // JWT'den admin bilgisini al ve debug için yazdır
                var adminClaim = User.FindFirst("IsAdmin")?.Value;
                Console.WriteLine($"Admin claim değeri: {adminClaim}");
                IsAdmin = adminClaim?.ToLower() == "true";
                Console.WriteLine($"IsAdmin property değeri: {IsAdmin}");

                // Kullanıcı ID'sini al ve debug için yazdır
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                Console.WriteLine($"User ID claim değeri: {userIdClaim}");

                await SetAuthorizationHeader();

                // Blog yazısını getir
                var blogResponse = await _httpClient.GetAsync($"/BlogPost/{id}");
                if (!blogResponse.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = "Blog yazısı bulunamadı.";
                    return RedirectToPage("/Index");
                }

                var blogContent = await blogResponse.Content.ReadAsStringAsync();
                Console.WriteLine($"Blog API yanıtı: {blogContent}"); // API yanıtını debug için yazdır

                var blogResult = JsonSerializer.Deserialize<Result<BlogPostDto>>(blogContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (blogResult?.Success == true)
                {
                    BlogPost = blogResult.Data;
                    Console.WriteLine($"Blog post user ID: {BlogPost.UserId}"); // Blog post user ID'sini debug için yazdır

                    // Eğer ImagePath sadece dosya adıysa, blog_photos/ ekle
                    if (!string.IsNullOrEmpty(BlogPost.ImagePath) && !BlogPost.ImagePath.Contains("/"))
                    {
                        BlogPost.ImagePath = $"/images/blog_photos/{BlogPost.ImagePath}";
                    }

                    // İlgili blog yazılarını getir
                    var relatedResponse = await _httpClient.GetAsync($"/BlogPost/related/{id}");
                    if (relatedResponse.IsSuccessStatusCode)
                    {
                        var relatedContent = await relatedResponse.Content.ReadAsStringAsync();
                        var relatedResult = JsonSerializer.Deserialize<Result<List<BlogPostDto>>>(relatedContent, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                        if (relatedResult?.Success == true)
                        {
                            RelatedPosts = relatedResult.Data;
                            // İlgili postların fotoğraf yollarını düzenle
                            foreach (var post in RelatedPosts)
                            {
                                if (!string.IsNullOrEmpty(post.ImagePath) && !post.ImagePath.Contains("/"))
                                {
                                    post.ImagePath = $"blog_photos/{post.ImagePath}";
                                }
                            }
                        }
                    }

                    // Yorumları ayrı bir endpoint'ten getir
                    var commentsResponse = await _httpClient.GetAsync($"/api/Comment/getbyblogpostid/{id}");
                    if (commentsResponse.IsSuccessStatusCode)
                    {
                        var commentsContent = await commentsResponse.Content.ReadAsStringAsync();
                        var commentsResult = JsonSerializer.Deserialize<Result<List<CommentDto>>>(commentsContent, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                        if (commentsResult?.Success == true)
                        {
                            Comments = commentsResult.Data;
                        }
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = blogResult?.Message ?? "Blog yazısı yüklenirken bir hata oluştu.";
                    return RedirectToPage("/Index");
                }

                return Page();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Bir hata oluştu: {ex.Message}";
                return RedirectToPage("/Index");
            }
        }

        public async Task<IActionResult> OnPostCommentAsync()
        {
            IsAdmin = User.FindFirst("IsAdmin")?.Value == "True";
            if (!ModelState.IsValid)
            {
                await OnGetAsync(NewComment.BlogPostId);
                return Page();
            }

            try
            {
                await SetAuthorizationHeader();
                var response = await _httpClient.PostAsJsonAsync("/api/Comment/add", NewComment);
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Yorumunuz başarıyla eklendi.";
                }
                else
                {
                    TempData["ErrorMessage"] = $"Yorum eklenirken bir hata oluştu: {content}";
                }

                return RedirectToPage("/Blog/Details", new { id = NewComment.BlogPostId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Yorum eklenirken bir hata oluştu: {ex.Message}";
                return RedirectToPage("/Blog/Details", new { id = NewComment.BlogPostId });
            }
        }

        public async Task<IActionResult> OnPostDeleteCommentAsync(int commentId)
        {
            IsAdmin = User.FindFirst("IsAdmin")?.Value == "True";
            try
            {
                await SetAuthorizationHeader();
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var isAdmin = User.FindFirst("IsAdmin")?.Value == "True";

                var response = await _httpClient.DeleteAsync($"/api/Comment/delete/{commentId}?userId={userId}&isAdmin={isAdmin}");

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Yorum başarıyla silindi.";
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = $"Yorum silinirken bir hata oluştu: {errorContent}";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Yorum silinirken bir hata oluştu: {ex.Message}";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditCommentAsync(int commentId, string content)
        {
            IsAdmin = User.FindFirst("IsAdmin")?.Value == "True";
            try
            {
                await SetAuthorizationHeader();
                var commentUpdateDto = new CommentUpdateDto
                {
                    Id = commentId,
                    Content = content
                };

                var response = await _httpClient.PutAsJsonAsync("/api/Comment/update", commentUpdateDto);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Yorum başarıyla güncellendi.";
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = $"Yorum güncellenirken bir hata oluştu: {errorContent}";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Yorum güncellenirken bir hata oluştu: {ex.Message}";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int blogId)
        {
            IsAdmin = User.FindFirst("IsAdmin")?.Value == "True";
            try
            {
                await SetAuthorizationHeader();
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var isAdmin = User.FindFirst("IsAdmin")?.Value == "True";
                
                var response = await _httpClient.DeleteAsync($"/BlogPost/{blogId}?userId={userId}&isAdmin={isAdmin}");
                var content = await response.Content.ReadAsStringAsync();
                
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Blog yazısı başarıyla silindi.";
                    return RedirectToPage("/Index");
                }
                else
                {
                    TempData["ErrorMessage"] = $"Silme işlemi başarısız oldu: {content}";
                    return RedirectToPage();
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Silme işlemi sırasında bir hata oluştu: {ex.Message}";
                return RedirectToPage();
            }
        }

        private async Task SetAuthorizationHeader()
        {
            var token = _httpContextAccessor.HttpContext.Request.Cookies["JwtToken"];
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }
    }
} 