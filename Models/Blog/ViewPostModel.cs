using Blog.Areas.Identity.Data;

namespace Blog.Models.Blog
{
    public class ViewPostModel
    {
        public Post Post { get; set; }
        public IEnumerable<Post> LatestPosts { get; set;}
        public IEnumerable<Post> SimilarPosts { get; set;}
    }
}
