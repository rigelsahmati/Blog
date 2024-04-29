using Blog.Areas.Identity.Data;

namespace Blog.Models
{
    public class FilterUserPosts
    {
        public int postId { get; set; }
        public string userId { get; set; }
        public string viewerId { get; set; }
        public string fullName { get; set; }
        public string title { get; set; }
        public string createdTime { get; set; }
        public string imageUrl { get; set; }
        public string profileImage { get; set; }
        public int likes { get; set; }
        public int coments { get; set; }
        public bool liked { get; set; }
    }
}
