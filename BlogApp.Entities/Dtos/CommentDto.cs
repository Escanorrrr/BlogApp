using System;
using System.ComponentModel.DataAnnotations;

namespace BlogApp.Entities.DTOs
{
    public class CommentDto
    {
        public int Id { get; set; }
        
        [Required]
        public string Content { get; set; }
        
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime CreatedAt { get; set; }
        
        [Required]
        public string UserName { get; set; }
        
        public int BlogPostId { get; set; }

        public int UserId { get; set; }
    }
} 