using System.ComponentModel.DataAnnotations;

namespace BlogApp.Entities.DTOs
{
    public class CommentUpdateDto
    {
        [Required(ErrorMessage = "Yorum ID'si zorunludur.")]
        [Range(1, int.MaxValue, ErrorMessage = "Geçersiz yorum ID'si.")]
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Yorum içeriği zorunludur.")]
        [StringLength(1000, MinimumLength = 2, ErrorMessage = "Yorum içeriği 2-1000 karakter arasında olmalıdır.")]
        public string Content { get; set; }
    }
} 