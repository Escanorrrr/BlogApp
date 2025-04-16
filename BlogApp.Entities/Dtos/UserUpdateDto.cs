using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp.Entities.Dtos
{
    public class UserUpdateDto
    {
        [Required(ErrorMessage = "ID alanı zorunludur.")]
        [Range(1, int.MaxValue, ErrorMessage = "Geçersiz ID.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Kullanıcı adı zorunludur.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Kullanıcı adı 3-50 karakter arasında olmalıdır.")]
        [RegularExpression(@"^[a-zA-Z0-9_-]+$", ErrorMessage = "Kullanıcı adı sadece harf, rakam, alt çizgi ve tire içerebilir.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-posta adresi zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        public string Email { get; set; } = string.Empty;
        
        /// <summary>
        /// Kullanıcının admin olup olmadığını belirtir.
        /// </summary>
        public bool IsAdmin { get; set; } = false;

        /// <summary>
        /// Kullanıcının profil fotoğrafı (base64 formatında)
        /// </summary>
        public string? ImageBase64 { get; set; }

        /// <summary>
        /// Kullanıcının profil fotoğrafı yolu
        /// </summary>
        public string? ImagePath { get; set; }
    }
} 