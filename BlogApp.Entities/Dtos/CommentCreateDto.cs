using System.ComponentModel.DataAnnotations;

namespace BlogApp.Entities.DTOs
{
    public class CommentCreateDto
    {
        [Required(ErrorMessage = "Yorum içeriği zorunludur.")]
        [StringLength(1000, MinimumLength = 2, ErrorMessage = "Yorum içeriği 2-1000 karakter arasında olmalıdır.")]
        public string Content { get; set; }
        
        [Required(ErrorMessage = "Blog yazısı ID'si zorunludur.")]
        [Range(1, int.MaxValue, ErrorMessage = "Geçerli bir blog yazısı seçmelisiniz.")]
        public int BlogPostId { get; set; }
    }
} 