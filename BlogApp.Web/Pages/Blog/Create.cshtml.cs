using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BlogApp.Entities.Dtos;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using BlogApp.Entities.Helpers;
using System.Net.Http.Headers;
using BlogApp.Entities;

namespace Web.Pages.Blog
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public CreateModel(IHttpClientFactory httpClientFactory, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _httpClient.BaseAddress = new Uri(configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5082");
        }

        [BindProperty]
        public BlogPostAddDto BlogPost { get; set; }

        public List<Category> Categories { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                await SetAuthorizationHeader();
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
                        Categories = result.Data;
                    }
                }
                return Page();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Kategoriler yüklenirken bir hata oluştu: {ex.Message}";
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }

            try
            {
                await SetAuthorizationHeader();

                // Blog yazısını oluştur
                var response = await _httpClient.PostAsJsonAsync("/BlogPost", BlogPost);
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Blog yazısı başarıyla oluşturuldu.";
                    return RedirectToPage("/Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, $"Blog yazısı oluşturulurken bir hata oluştu: {content}");
                    await OnGetAsync();
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Blog yazısı oluşturulurken bir hata oluştu: {ex.Message}");
                await OnGetAsync();
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
    }
} 