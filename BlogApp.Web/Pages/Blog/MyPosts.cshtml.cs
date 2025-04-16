using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BlogApp.Entities.Dtos;
using System.Text.Json;
using BlogApp.Entities.Helpers;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Web.Pages.Blog
{
    [Authorize]
    public class MyPostsModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public List<BlogPostDto> BlogPosts { get; set; } = new();

        public MyPostsModel(IHttpClientFactory httpClientFactory, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _httpClient.BaseAddress = new Uri(configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5082");
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                await SetAuthorizationHeader();
                var response = await _httpClient.GetAsync("/BlogPost/my-posts");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<Result<List<BlogPostDto>>>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (result?.Success == true)
                    {
                        BlogPosts = result.Data;
                    }
                    else
                    {
                        TempData["ErrorMessage"] = result?.Message ?? "Blog yazıları yüklenirken bir hata oluştu.";
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = $"Blog yazıları yüklenirken bir hata oluştu: {errorContent}";
                }

                return Page();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Bir hata oluştu: {ex.Message}";
                return Page();
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

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            try
            {
                await SetAuthorizationHeader();
                var response = await _httpClient.DeleteAsync($"/BlogPost/{id}");
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Blog yazısı başarıyla silindi.";
                }
                else
                {
                    TempData["ErrorMessage"] = $"Blog yazısı silinirken bir hata oluştu: {content}";
                }

                return RedirectToPage();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Blog yazısı silinirken bir hata oluştu: {ex.Message}";
                return RedirectToPage();
            }
        }
    }
} 
