using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlogApp.Entities.DTOs;

namespace BlogApp.Entities.Dtos
{
    public class BlogPostDto
    {
        public int Id { get; set; }
        
        [Required]
        public string Title { get; set; }
        
        [Required]
        public string Content { get; set; }
        
        public string? ImagePath { get; set; }
        
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime PublishedDate { get; set; }
        
        [Required]
        public string CategoryName { get; set; }

        [Required]
        public int CategoryId { get; set; }
        
        [Required]
        public string AuthorUsername { get; set; }

        [Required]
        public int UserId { get; set; }

        public List<CommentDto> Comments { get; set; } = new();
    }
}

