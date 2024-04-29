using Blog.Areas.Identity.Data;

namespace Blog.Models.Admin
{
    public class ViewUserModel
    {
        public User User { get; set; }
        public IEnumerable<Role> Roles { get; set; }
        public IEnumerable<Post> ViewerLikedPosts { get; set; }
        public IEnumerable<Categorie> Categories { get; set; }
        public string AdminId { get; set; }
        public int RoleId { get; set; }
    }
}
