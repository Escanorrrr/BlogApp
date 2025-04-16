using BlogApp.Business.Services.Abstract;
using BlogApp.Entities.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BlogApp.Business.Helpers;
using BlogApp.Entities.Helpers;

namespace BlogApp.Controllers
{
    /// <summary>
    /// Yorum yönetimi için API endpoints sunan controller.
    /// Yorum ekleme, güncelleme, silme ve listeleme işlemlerini yönetir.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;

        /// <summary>
        /// CommentController yapıcı metodu.
        /// </summary>
        /// <param name="commentService">Yorum işlemleri servis arayüzü</param>
        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }
        
        /// <summary>
        /// Belirtilen ID'ye sahip yorumu getirir.
        /// </summary>
        /// <param name="id">Yorum ID'si</param>
        /// <returns>Yorum detayları</returns>
        [AllowAnonymous]
        [HttpGet("getbyid/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _commentService.GetByIdAsync(id);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        
        /// <summary>
        /// Tüm yorumları listeler. Sadece admin yetkisi ile erişilebilir.
        /// </summary>
        /// <returns>Tüm yorumların listesi</returns>
        [Authorize(Policy = "AdminPolicy")]
        [HttpGet("getall")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _commentService.GetAllAsync();
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        
        /// <summary>
        /// Belirli bir blog yazısına ait yorumları getirir.
        /// </summary>
        /// <param name="blogPostId">Blog yazısı ID'si</param>
        /// <returns>Belirtilen blog yazısına ait yorumların listesi</returns>
        [AllowAnonymous]
        [HttpGet("getbyblogpostid/{blogPostId}")]
        public async Task<IActionResult> GetByBlogPostId(int blogPostId)
        {
            var result = await _commentService.GetByBlogPostIdAsync(blogPostId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Yeni bir yorum ekler.
        /// </summary>
        /// <param name="commentCreateDto">Eklenecek yorum bilgileri</param>
        /// <returns>Eklenen yorum bilgileri</returns>
        [HttpPost("add")]
        public async Task<IActionResult> Add(CommentCreateDto commentCreateDto)
        {
            int? userId = UserHelper.GetLoggedInUserId(HttpContext);
            
            var result = await _commentService.AddAsync(commentCreateDto, userId.Value);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Mevcut bir yorumu günceller.
        /// </summary>
        /// <param name="commentUpdateDto">Güncellenecek yorum bilgileri</param>
        /// <returns>Güncellenen yorum bilgileri</returns>
        [HttpPut("update")]
        public async Task<IActionResult> Update(CommentUpdateDto commentUpdateDto)
        {
            int? userId = UserHelper.GetLoggedInUserId(HttpContext);
            
            var result = await _commentService.UpdateAsync(commentUpdateDto, userId.Value);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Belirtilen ID'ye sahip yorumu siler.
        /// </summary>
        /// <param name="id">Silinecek yorum ID'si</param>
        /// <returns>Silme işlemi sonucu</returns>
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            int? userId = UserHelper.GetLoggedInUserId(HttpContext);
            
            var result = await _commentService.DeleteAsync(id, userId.Value);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
} 