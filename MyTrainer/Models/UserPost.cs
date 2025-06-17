namespace MyTrainer.Models
{
    public class UserPost
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public string content { get; set; }
        public string? imageUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
