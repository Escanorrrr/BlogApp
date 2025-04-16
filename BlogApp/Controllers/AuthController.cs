using Microsoft.AspNetCore.Mvc;
using BlogApp.Business.Services.Abstract;
using BlogApp.Entities.Dtos;
using Microsoft.Extensions.Configuration;
using BlogApp.Business.Helpers;

namespace BlogApp.Controllers
{
    /// <summary>
    /// Kullanıcı kimlik doğrulama ve kayıt işlemleri için API endpoints.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// AuthController yapıcı metodu.
        /// </summary>
        /// <param name="userService">Kullanıcı işlemleri servis arayüzü</param>
        /// <param name="configuration">Yapılandırma bilgisi</param>
        public AuthController(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }

        /// <summary>
        /// Yeni bir kullanıcı kaydı oluşturur.
        /// </summary>
        /// <param name="dto">Kayıt için kullanıcı bilgileri</param>
        /// <returns>Kayıt işlemi sonucu ve kullanıcı bilgileri</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var result = await _userService.RegisterAsync(dto);

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new
            {
                message = result.Message,
                user = new
                {
                    result.Data.Id,
                    result.Data.Username,
                    result.Data.Email
                }
            });
        }

        /// <summary>
        /// Kullanıcı girişi yapar ve JWT token döner.
        /// </summary>
        /// <param name="dto">Giriş bilgileri</param>
        /// <returns>JWT token ve kullanıcı bilgileri</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var result = await _userService.AuthenticateAsync(dto);
            if (!result.Success)
                return Unauthorized(result.Message);

            var jwtSection = _configuration.GetSection("Jwt");
            var secretKey = _configuration.GetValue<string>("Jwt:SecretKey");

            var token = TokenHelper.GenerateToken(result.Data!, _configuration);

            return Ok(new
            {
                token,
                user = new
                {
                    result.Data.Id,
                    result.Data.Username,
                    result.Data.Email
                }
            });
        }
    }
}
