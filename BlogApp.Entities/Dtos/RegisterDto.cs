using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace BlogApp.Entities.Dtos
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Kullanıcı adı zorunludur.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Kullanıcı adı 3-50 karakter arasında olmalıdır.")]
        [RegularExpression(@"^[a-zA-Z0-9_-]+$", ErrorMessage = "Kullanıcı adı sadece harf, rakam, alt çizgi ve tire içerebilir.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email alanı zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz.")]
        [StringLength(100, ErrorMessage = "Email en fazla 100 karakter olabilir.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifre alanı zorunludur.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Şifre 6-100 karakter arasında olmalıdır.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{6,}$", 
            ErrorMessage = "Şifre en az bir küçük harf, bir büyük harf, bir rakam ve bir özel karakter içermelidir.")]
        public string Password { get; set; }
        
        /// <summary>
        /// Kullanıcının admin olup olmadığını belirtir.
        /// Bu değer her zaman false olarak ayarlanır ve API tarafında dikkate alınmaz.
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore] // API isteklerinde bu alanı yok sayacaktır
        public bool IsAdmin { get; set; } = false;

        /// <summary>
        /// Kullanıcı fotoğrafı (base64 formatında)
        /// </summary>
        public string? ImageBase64 { get; set; }
    }
}

