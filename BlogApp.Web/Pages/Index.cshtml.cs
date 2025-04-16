using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BlogApp.Entities.Dtos;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text.Json;
using BlogApp.Entities.Helpers;

namespace BlogApp.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public List<BlogPostDto> BlogPosts { get; set; } = new();

        public IndexModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri(configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5082");
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/BlogPost");
                
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
                        ModelState.AddModelError(string.Empty, result?.Message ?? "Blog yazıları yüklenirken bir hata oluştu.");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Blog yazıları yüklenirken bir hata oluştu.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Bir hata oluştu: {ex.Message}");
            }

            return Page();
        }
    }
}
