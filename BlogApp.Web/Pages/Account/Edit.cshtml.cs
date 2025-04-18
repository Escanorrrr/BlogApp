using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Net.Http.Headers;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Web.Models.Responses;
using BlogApp.Entities.Dtos;

namespace Web.Pages.Account
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        [BindProperty]
        public UserUpdateDto UserUpdateDto { get; set; }

        public EditModel(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
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
                await SetAuthorizationHeader();
                var response = await _httpClient.GetAsync($"api/User/{userId}");
                var responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Get User Response: {responseContent}"); // Debug için

                if (!response.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = $"Kullanıcı bilgileri alınamadı: {responseContent}";
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

                UserUpdateDto = new UserUpdateDto
                {
                    Id = int.Parse(userId),
                    Username = result.Data.Username,
                    Email = result.Data.Email,
                    IsAdmin = result.Data.IsAdmin
                };

                return Page();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Bir hata oluştu: " + ex.Message;
                return RedirectToPage("/Account/Profile");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToPage("/Account/Login");
            }

            try
            {
                // UserUpdateDto'ya ID'yi ekle
                UserUpdateDto.Id = int.Parse(userId);

                // Debug için request verilerini logla
                Console.WriteLine($"Request URL: {_httpClient.BaseAddress}api/User/{userId}");
                Console.WriteLine($"Request Data: {JsonSerializer.Serialize(UserUpdateDto)}");

                await SetAuthorizationHeader();
                var response = await _httpClient.PutAsJsonAsync($"api/User/{userId}", UserUpdateDto);
                var responseContent = await response.Content.ReadAsStringAsync();
                
                // Debug için response'u logla
                Console.WriteLine($"Response Status: {response.StatusCode}");
                Console.WriteLine($"Response Content: {responseContent}");

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
                Console.WriteLine($"Exception: {ex.Message}"); // Debug için
                Console.WriteLine($"Stack Trace: {ex.StackTrace}"); // Debug için
                
                ModelState.AddModelError(string.Empty, "Bir hata oluştu: " + ex.Message);
                return Page();
            }
        }

        private async Task SetAuthorizationHeader()
        {
            var token = _httpContextAccessor.HttpContext.Request.Cookies["token"];
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }
    }

    public class ApiResponse<T>
    {
        public T Data { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }
    }
} 