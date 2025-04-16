using BlogApp.Entities.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;
using System.Text.Json;

namespace Web.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpClient _httpClient;

        public RegisterModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _httpClient = _httpClientFactory.CreateClient("BlogApi");
        }

        [BindProperty]
        public RegisterDto RegisterDto { get; set; } = new();

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        Console.WriteLine($"ModelState Error: {error.ErrorMessage}");
                    }
                }
                return Page();
            }

            try
            {
                var endpoint = "Auth/register";
                Console.WriteLine($"Request URL: {_httpClient.BaseAddress}{endpoint}");
                Console.WriteLine($"Request Data: {JsonSerializer.Serialize(RegisterDto)}");

                var response = await _httpClient.PostAsJsonAsync(endpoint, RegisterDto);
                var responseContent = await response.Content.ReadAsStringAsync();
                
                Console.WriteLine($"Response Status: {response.StatusCode}");
                Console.WriteLine($"Response Content: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage("/Account/Login");
                }
                
                try 
                {
                    var error = JsonSerializer.Deserialize<Dictionary<string, object>>(responseContent);
                    ModelState.AddModelError(string.Empty, error?["message"]?.ToString() ?? "Kayıt işlemi başarısız oldu.");
                }
                catch
                {
                    ModelState.AddModelError(string.Empty, responseContent);
                }
                return Page();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                
                ModelState.AddModelError(string.Empty, "Bir hata oluştu: " + ex.Message);
                return Page();
            }
        }
    }
}
