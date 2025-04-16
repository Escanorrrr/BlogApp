using BlogApp.Business.Services.Abstract;
using BlogApp.Entities.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BlogApp.Business.Helpers;
using BlogApp.Entities.Helpers;

namespace BlogApp.Controllers
{
    /// <summary>
    /// Blog yazıları yönetimi için API endpoints sunan controller.
    /// Blog yazısı ekleme, güncelleme, silme ve listeleme işlemlerini yönetir.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class BlogPostController : ControllerBase
    {
        private readonly IBlogPostService _blogPostService;

        /// <summary>
        /// BlogPostController yapıcı metodu.
        /// </summary>
        /// <param name="blogPostService">Blog yazıları işlemleri servis arayüzü</param>
        public BlogPostController(IBlogPostService blogPostService)
        {
            _blogPostService = blogPostService;
        }

        /// <summary>
        /// Tüm blog yazılarını getirir.
        /// </summary>
        /// <returns>Blog yazılarının listesi</returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _blogPostService.GetAllAsync();
            if(!result.Success)
            {
                return StatusCode(400, result);
            }
            return Ok(result);
        }

        /// <summary>
        /// ID'ye göre blog yazısı getirir.
        /// </summary>
        /// <param name="id">Blog yazısının ID'si</param>
        /// <returns>Blog yazısı</returns>
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _blogPostService.GetByIdAsync(id);
            if(!result.Success)
            {
                return StatusCode(404, result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Blog yazısını günceller.
        /// ImageBase64 alanı ile resim yükleyebilirsiniz.
        /// </summary>
        /// <param name="dto">Güncellenecek blog yazısı bilgileri</param>
        /// <returns>Güncellenen blog yazısı</returns>
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] BlogPostUpdateDto dto)
        {
            int? userId = UserHelper.GetLoggedInUserId(HttpContext);
            bool isAdmin = UserHelper.IsLoggedInUserAdmin(HttpContext);

            var result = await _blogPostService.UpdateAsync(dto, userId.Value, isAdmin);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            
            return Ok(result);
        }

        /// <summary>
        /// Blog yazısını siler.
        /// </summary>
        /// <param name="id">Silinecek blog yazısının ID'si</param>
        /// <returns>İşlem sonucu</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            int? userId = UserHelper.GetLoggedInUserId(HttpContext);
            bool isAdmin = UserHelper.IsLoggedInUserAdmin(HttpContext);

            var result = await _blogPostService.DeleteAsync(id, userId.Value, isAdmin);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            
            return Ok(result);
        }

        /// <summary>
        /// Yeni bir blog yazısı ekler.
        /// ImageBase64 alanı ile resim yükleyebilirsiniz.
        /// </summary>
        /// <param name="dto">Eklenecek blog yazısı bilgileri</param>
        /// <returns>Eklenen blog yazısı</returns>
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] BlogPostAddDto dto)
        {
            int? userId = UserHelper.GetLoggedInUserId(HttpContext);

            var result = await _blogPostService.AddAsync(dto, userId.Value);
            if (!result.Success)
            {
                return StatusCode(400, result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Belirli bir kategoriye ait blog yazılarını getirir.
        /// </summary>
        /// <param name="categoryId">Kategori ID'si</param>
        /// <returns>Belirtilen kategoriye ait blog yazıları listesi</returns>
        [AllowAnonymous]
        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetByCategory(int categoryId)
        {
            var result = await _blogPostService.GetByCategoryIdAsync(categoryId);
            if (!result.Success)
            {
                return StatusCode(400, result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Giriş yapmış kullanıcının blog yazılarını getirir.
        /// </summary>
        /// <returns>Kullanıcının blog yazıları listesi</returns>
        [Authorize]
        [HttpGet("my-posts")]
        public async Task<IActionResult> GetMyPosts()
        {
            int? userId = UserHelper.GetLoggedInUserId(HttpContext);
            var result = await _blogPostService.GetByUserIdAsync(userId.Value);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
