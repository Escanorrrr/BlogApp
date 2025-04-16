using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using BlogApp.Entities.Dtos;
using BlogApp.Entities;
using System.Text.Json;
using BlogApp.Entities.Helpers;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;

namespace Web.Pages.Blog
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        [BindProperty]
        public BlogPostUpdateDto BlogPost { get; set; }

        public SelectList Categories { get; set; }
        public string CurrentImagePath { get; set; }

        public EditModel(IHttpClientFactory httpClientFactory, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri(configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5082");
            _httpContextAccessor = httpContextAccessor;
        }

        private void SetAuthorizationHeader()
        {
            var token = _httpContextAccessor.HttpContext?.Request.Cookies["JwtToken"];
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        private async Task LoadCategoriesAsync()
        {
            try
            {
                SetAuthorizationHeader();
                var response = await _httpClient.GetAsync("/Category");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<Result<List<Category>>>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (result?.Success == true)
                    {
                        Categories = new SelectList(result.Data, "Id", "Name");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Kategoriler yüklenirken bir hata oluştu.");
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Kategoriler yüklenirken bir hata oluştu: {ex.Message}");
            }
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                SetAuthorizationHeader();
                await LoadCategoriesAsync();

                var response = await _httpClient.GetAsync($"/BlogPost/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = "Blog yazısı bulunamadı.";
                    return RedirectToPage("/Index");
                }

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<Result<BlogPostDto>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (result?.Success == true)
                {
                    BlogPost = new BlogPostUpdateDto
                    {
                        Id = result.Data.Id,
                        Title = result.Data.Title,
                        Content = result.Data.Content,
                        CategoryId = result.Data.CategoryId
                    };
                    CurrentImagePath = result.Data.ImagePath;
                    return Page();
                }

                TempData["ErrorMessage"] = result?.Message ?? "Blog yazısı bulunamadı.";
                return RedirectToPage("/Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Bir hata oluştu: {ex.Message}";
                return RedirectToPage("/Index");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync();
                return Page();
            }

            try
            {
                SetAuthorizationHeader();
                // Debug için request body'yi logla
                Console.WriteLine($"Request Body: {JsonSerializer.Serialize(BlogPost)}");
                
                var response = await _httpClient.PutAsJsonAsync("/BlogPost", BlogPost);
                
                // Debug için response'u logla
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response Status: {response.StatusCode}");
                Console.WriteLine($"Response Content: {responseContent}");
                
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<Result<BlogPostDto>>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (result?.Success == true)
                    {
                        TempData["SuccessMessage"] = "Blog yazısı başarıyla güncellendi.";
                        return RedirectToPage("/Blog/Details", new { id = BlogPost.Id });
                    }
                    
                    ModelState.AddModelError(string.Empty, result?.Message ?? "Güncelleme sırasında bir hata oluştu.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, $"Güncelleme sırasında bir hata oluştu. Status: {response.StatusCode}, Content: {responseContent}");
                }

                await LoadCategoriesAsync();
                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Bir hata oluştu: {ex.Message}");
                await LoadCategoriesAsync();
                return Page();
            }
        }
    }
} 