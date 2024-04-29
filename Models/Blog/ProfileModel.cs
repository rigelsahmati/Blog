using Blog.Areas.Identity.Data;

namespace Blog.Models.Blog
{
    public class ProfileModel
    {
        public User User { get; set; }
        public IEnumerable<Post> LikedPosts { get; set; }
        public IEnumerable<Categorie> Categories { get; set; }
    }
}
