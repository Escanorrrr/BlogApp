using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BlogApp.Entities.Dtos;
using BlogApp.Entities.Helpers;
using System.Text.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlogApp.Web.Pages.Blog
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;
        public List<BlogPostDto> BlogPosts { get; set; }

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("BlogApi");
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var response = await _httpClient.GetAsync("api/BlogPost");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<Result<List<BlogPostDto>>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (result.Success)
                {
                    BlogPosts = result.Data;
                    return Page();
                }
            }

            return RedirectToPage("/Error");
        }
    }
} 