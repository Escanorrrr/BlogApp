﻿namespace BlogApp.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        

        public int BlogPostId { get; set; }
        public BlogPost BlogPost { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }

}
