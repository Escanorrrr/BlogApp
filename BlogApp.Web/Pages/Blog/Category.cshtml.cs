using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BlogApp.Entities.Dtos;
using System.Text.Json;
using Web.Models.Responses;

namespace Web.Pages.Blog
{
    public class CategoryModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public List<BlogPostDto> Posts { get; set; } = new();
        public string CategoryName { get; set; } = string.Empty;

        public CategoryModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient("BlogApi");
            _configuration = configuration;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                // Önce kategori bilgisini al
                var categoryResponse = await _httpClient.GetAsync($"/Category/{id}");
                if (categoryResponse.IsSuccessStatusCode)
                {
                    var categoryContent = await categoryResponse.Content.ReadAsStringAsync();
                    var categoryResult = JsonSerializer.Deserialize<ApiResponse<CategoryDto>>(categoryContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (categoryResult?.Success == true)
                    {
                        CategoryName = categoryResult.Data.Name;
                    }
                }

                // Kategoriye ait postları al
                var response = await _httpClient.GetAsync($"/BlogPost/category/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    return NotFound();
                }

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponse<List<BlogPostDto>>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (result?.Success == true)
                {
                    Posts = result.Data;
                }

                return Page();
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error");
            }
        }
    }

    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
} 