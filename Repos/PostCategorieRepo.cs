using Blog.Areas.Identity.Data;
using Blog.Data;
namespace Blog.Repos
{
    public class PostCategorieRepo : IPostCategorieRepo
    {
        private readonly BlogContext _context;
        public PostCategorieRepo(BlogContext context) { _context = context; }
        public async Task CreatePostRepo(int postId, int categorieId)
        {
                var pc = new PostCategorie { PostId = postId, CategorieId = categorieId };
                await _context.PostCategories.AddAsync(pc);
                await _context.SaveChangesAsync();
        }
    }
}
