namespace BlogApp.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? ImagePath { get; set; }
        public bool IsBlocked { get; set; }
        public bool IsAdmin { get; set; } = false;

        public ICollection<BlogPost> BlogPosts { get; set; }
        public ICollection<Comment> Comments { get; set; }
    }

}
