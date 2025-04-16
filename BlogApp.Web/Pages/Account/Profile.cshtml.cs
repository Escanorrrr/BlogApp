using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using BlogApp.Entities.DTOs;
using BlogApp.Entities.Helpers;
using System.Security.Claims;
using BlogApp.Entities.Dtos;

namespace Web.Pages.Account
{
    public class ProfileModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public UserDto UserData { get; set; }
        public bool IsAdmin { get; set; }

        public ProfileModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _configuration = configuration;
            _httpClient.BaseAddress = new Uri(configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5082");
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var currentUser = (ClaimsPrincipal)User;
                var userId = currentUser.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                IsAdmin = currentUser.FindFirst("IsAdmin")?.Value == "True";

                if (string.IsNullOrEmpty(userId))
                {
                    return RedirectToPage("/Account/Login");
                }

                var token = HttpContext.Request.Cookies["JwtToken"];
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                Console.WriteLine($"Base URL: {_httpClient.BaseAddress}");
                Console.WriteLine($"User ID: {userId}");
                
                var response = await _httpClient.GetAsync($"api/User/{userId}");
                Console.WriteLine($"Response Status: {response.StatusCode}");
                
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response Content: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<Result<UserDto>>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (result?.Success == true)
                    {
                        UserData = result.Data;
                        return Page();
                    }
                }
                
                TempData["ErrorMessage"] = $"Kullanıcı bilgileri alınamadı. Status: {response.StatusCode}, Content: {responseContent}";
                return RedirectToPage("/Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Bir hata oluştu: {ex.Message}";
                return RedirectToPage("/Index");
            }
        }
    }
} 