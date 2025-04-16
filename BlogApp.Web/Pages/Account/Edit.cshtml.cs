using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using BlogApp.Entities.DTOs;
using System.Security.Claims;
using BlogApp.Business.Services.Abstract;

using BlogApp.Entities.Dtos;
using BlogApp.Entities.Helpers;

namespace BlogApp.Web.Pages.Account
{
    public class EditModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IFotoService _fotoService;

        [BindProperty]
        public UserUpdateDto UserUpdateDto { get; set; }
        public string CurrentImagePath { get; set; }

        public EditModel(IHttpClientFactory httpClientFactory, IFotoService fotoService)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _fotoService = fotoService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToPage("/Account/Login");
            }

            var response = await _httpClient.GetAsync($"api/User/{userId}");
            if (!response.IsSuccessStatusCode)
            {
                return RedirectToPage("/Error");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<Result<UserDto>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (!result.Success)
            {
                return RedirectToPage("/Error");
            }

            UserUpdateDto = new UserUpdateDto
            {
                Id = int.Parse(userId),
                Username = result.Data.Username,
                Email = result.Data.Email,
                IsAdmin = result.Data.IsAdmin
            };

            CurrentImagePath = result.Data.ImagePath;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToPage("/Account/Login");
            }

            // Eğer yeni bir fotoğraf yüklendiyse
            if (!string.IsNullOrEmpty(UserUpdateDto.ImageBase64))
            {
                var uploadResult = await _fotoService.UploadAsync(UserUpdateDto.ImageBase64);
                if (uploadResult.Success)
                {
                    UserUpdateDto.ImagePath = uploadResult.Data;
                }
                else
                {
                    ModelState.AddModelError("", "Fotoğraf yüklenirken bir hata oluştu.");
                    return Page();
                }
            }

            var response = await _httpClient.PutAsJsonAsync($"api/User/update/{userId}", UserUpdateDto);
            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Response Content: {responseContent}"); // Debug için

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Profiliniz başarıyla güncellendi.";
                return RedirectToPage("/Account/Profile");
            }
            else
            {
                ModelState.AddModelError("", "Profil güncellenirken bir hata oluştu.");
                return Page();
            }
        }
    }
} 