using Blog.Areas.Identity.Data;

namespace Blog.Models.Admin
{
    public class ViewPostsModel
    {
        public string PageTitle { get; set; }
        public User User {get; set;} 
        public IEnumerable<Post> Posts { get; set; }
        public IEnumerable<Categorie> Categories { get; set; }
    }
}
