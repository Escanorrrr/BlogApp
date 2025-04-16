using System.Security.Claims;
using BlogApp.Business.Services.Abstract;
using BlogApp.Entities.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BlogApp.Business.Helpers;
using BlogApp.Entities.Helpers;

namespace BlogApp.Controllers
{
    /// <summary>
    /// Kullanıcı yönetimi için API endpoints sunan controller.
    /// Kullanıcı oluşturma, güncelleme, silme ve listeleme işlemlerini yönetir.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        /// <summary>
        /// UserController yapıcı metodu.
        /// </summary>
        /// <param name="userService">Kullanıcı işlemleri servis arayüzü</param>
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Belirtilen ID'ye sahip kullanıcıyı getirir.
        /// </summary>
        /// <param name="id">Kullanıcı ID'si</param>
        /// <returns>Kullanıcı detayları</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            // UserHelper sınıfını kullanarak admin kontrolü
            bool isAdmin = UserHelper.IsLoggedInUserAdmin(HttpContext);
            int? userId = UserHelper.GetLoggedInUserId(HttpContext);
            // Kullanıcı admin değilse ve kendi ID'sinden farklı bir ID'ye erişmeye çalışıyorsa
            if (!isAdmin && userId != id)
            {
                return StatusCode(403, Result<object>.FailureResult("Sadece kendi bilgilerinizi görüntüleyebilirsiniz."));
            }

            var result = await _userService.GetByIdAsync(id);
            if (!result.Success)
            {
                return StatusCode(404, result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Tüm kullanıcıları listeler.
        /// </summary>
        /// <returns>Kullanıcı listesi</returns>
        [HttpGet]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _userService.GetAllAsync();
            if (!result.Success)
            {
                return StatusCode(400, result);
            }
            
            return Ok(result);
        }

        /// <summary>
        /// Yeni bir kullanıcı ekler.
        /// </summary>
        /// <param name="userDto">Eklenecek kullanıcı bilgileri</param>
        /// <returns>Eklenen kullanıcı bilgileri</returns>
        [HttpPost]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> Add([FromBody] UserAddDto userAddDto)
        {
            var result = await _userService.AddAsync(userAddDto);
            if (!result.Success)
            {
                return StatusCode(400, result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Mevcut bir kullanıcıyı günceller.
        /// </summary>
        /// <param name="id">Güncellenecek kullanıcı ID'si</param>
        /// <param name="userDto">Güncellenmiş kullanıcı bilgileri</param>
        /// <returns>Güncellenmiş kullanıcı bilgileri</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UserUpdateDto userUpdateDto)
        {
            // UserHelper sınıfını kullanarak admin kontrolü
            bool isAdmin = UserHelper.IsLoggedInUserAdmin(HttpContext);
            int? userId = UserHelper.GetLoggedInUserId(HttpContext);

            // Kullanıcı admin değilse ve kendi ID'sinden farklı bir ID'ye erişmeye çalışıyorsa
            if (!isAdmin && userId != id)
            {
                return StatusCode(403, Result<object>.FailureResult("Sadece kendi bilgilerinizi güncelleyebilirsiniz."));
            }

            if (id != userUpdateDto.Id)
                return StatusCode(400, Result<object>.FailureResult("Güncellenen kullanıcı ID'si, yol parametresi ile eşleşmiyor."));

            // Admin kontrolünü UserManager'a iletiyoruz
            var result = await _userService.UpdateAsync(userUpdateDto, isAdmin);
            if (!result.Success)
            {
                return StatusCode(400, result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Belirtilen ID'ye sahip kullanıcıyı siler.
        /// </summary>
        /// <param name="id">Silinecek kullanıcı ID'si</param>
        /// <returns>Silme işlemi sonucu</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            // UserHelper sınıfını kullanarak admin kontrolü
            bool isAdmin = UserHelper.IsLoggedInUserAdmin(HttpContext);
            int? userId = UserHelper.GetLoggedInUserId(HttpContext);
            
            // Kullanıcı admin değilse ve kendi hesabı dışında bir hesabı silmeye çalışıyorsa
            if (!isAdmin && userId != id)
            {
                return StatusCode(403, Result<object>.FailureResult("Sadece kendi hesabınızı silebilirsiniz."));
            }

            // UserService'den dönen sonucu al ve kontrol et
            var result = await _userService.DeleteAsync(id);
            if (!result.Success)
            {
                return StatusCode(400, result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Belirtilen ID'ye sahip kullanıcıyı yasaklar.
        /// </summary>
        /// <param name="id">Yasaklanacak kullanıcı ID'si</param>
        /// <returns>Yasaklama işlemi sonucu</returns>
        [HttpPost("ban/{id}")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> BanUser(int id)
        {
            var result = await _userService.BanUserAsync(id);
            if (!result.Success)
            {
                return StatusCode(400, result);
            }
            
            return Ok(result);
        }
    }
}
