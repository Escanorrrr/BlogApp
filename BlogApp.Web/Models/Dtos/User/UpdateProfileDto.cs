using System.ComponentModel.DataAnnotations;

namespace Web.Models.Dtos.User
{
    public class UpdateProfileDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kullanıcı adı zorunludur.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Kullanıcı adı 3-50 karakter arasında olmalıdır.")]
        [RegularExpression(@"^[a-zA-Z0-9_-]+$", ErrorMessage = "Kullanıcı adı sadece harf, rakam, alt çizgi ve tire içerebilir.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "E-posta adresi zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        public string Email { get; set; }

        public string? ImageBase64 { get; set; }
    }
} 