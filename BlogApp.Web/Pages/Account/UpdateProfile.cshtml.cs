using BlogApp.Entities.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using System.Text.Json;
using Web.Models.Dtos.User;
using Web.Models.Responses;

namespace Web.Pages.Account
{
    [Authorize]
    public class UpdateProfileModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        [BindProperty]
        public UpdateProfileDto ProfileDto { get; set; }

        public UpdateProfileModel(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClientFactory.CreateClient("BlogApi");
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToPage("/Account/Login");
            }

            try
            {
                var token = _httpContextAccessor.HttpContext.Request.Cookies["token"];
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                var response = await _httpClient.GetAsync($"api/User/{userId}");
                var responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"API Response: {responseContent}"); // Debug log

                if (!response.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = "Kullanıcı bilgileri alınamadı.";
                    return RedirectToPage("/Account/Profile");
                }

                var result = JsonSerializer.Deserialize<ApiResponse<UserDto>>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (result?.Data == null)
                {
                    TempData["ErrorMessage"] = "Kullanıcı bilgileri alınamadı.";
                    return RedirectToPage("/Account/Profile");
                }

                ProfileDto = new UpdateProfileDto
                {
                    Id = result.Data.Id,
                    Username = result.Data.Username,
                    Email = result.Data.Email,
                   
                };

                return Page();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}"); // Debug log
                TempData["ErrorMessage"] = $"Bir hata oluştu: {ex.Message}";
                return RedirectToPage("/Account/Profile");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var token = _httpContextAccessor.HttpContext.Request.Cookies["token"];
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                Console.WriteLine($"Sending update request for user ID: {ProfileDto.Id}"); // Debug log
                Console.WriteLine($"Image Base64 length: {ProfileDto.ImageBase64?.Length ?? 0}"); // Debug log

                var response = await _httpClient.PutAsJsonAsync($"api/User/update/{ProfileDto.Id}", ProfileDto);
                var responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Update Response: {responseContent}"); // Debug log

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Profil başarıyla güncellendi.";
                    return RedirectToPage("/Account/Profile");
                }

                var error = JsonSerializer.Deserialize<ApiResponse<object>>(responseContent);
                ModelState.AddModelError(string.Empty, error?.Message ?? "Profil güncellenirken bir hata oluştu.");
                return Page();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Update Error: {ex.Message}"); // Debug log
                Console.WriteLine($"Stack Trace: {ex.StackTrace}"); // Debug log
                ModelState.AddModelError(string.Empty, $"Bir hata oluştu: {ex.Message}");
                return Page();
            }
        }
    }
} 