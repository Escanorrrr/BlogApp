using System.ComponentModel.DataAnnotations;

namespace BlogApp.Entities.Dtos
{
    public class BlogPostAddDto
    {
        [Required(ErrorMessage = "Başlık zorunludur.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Başlık 3-100 karakter arasında olmalıdır.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "İçerik zorunludur.")]
        [StringLength(10000, MinimumLength = 10, ErrorMessage = "İçerik en az 10 karakter olmalıdır.")]
        public string Content { get; set; }

        [Required(ErrorMessage = "Kategori seçimi zorunludur.")]
        [Range(1, int.MaxValue, ErrorMessage = "Geçerli bir kategori seçmelisiniz.")]
        public int CategoryId { get; set; }

        public string? ImageBase64 { get; set; }
        
    }
}

