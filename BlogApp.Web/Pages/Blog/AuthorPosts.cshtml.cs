using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BlogApp.Entities.Dtos;
using BlogApp.Business.Services.Abstract;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Web.Pages.Blog
{
    public class AuthorPostsModel : PageModel
    {
        private readonly IBlogPostService _blogPostService;
        private readonly IUserService _userService;

        public AuthorPostsModel(IBlogPostService blogPostService, IUserService userService)
        {
            _blogPostService = blogPostService;
            _userService = userService;
        }

        public List<BlogPostDto> BlogPosts { get; set; }
        public string AuthorName { get; set; }

        public async Task<IActionResult> OnGetAsync(int authorId)
        {
            var userResult = await _userService.GetByIdAsync(authorId);
            if (!userResult.Success)
            {
                TempData["ErrorMessage"] = "Yazar bulunamadı.";
                return RedirectToPage("/Index");
            }

            AuthorName = userResult.Data.Username;

            var result = await _blogPostService.GetByUserIdAsync(authorId);
            if (result.Success)
            {
                BlogPosts = result.Data;
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
                BlogPosts = new List<BlogPostDto>();
            }

            return Page();
        }
    }
} 