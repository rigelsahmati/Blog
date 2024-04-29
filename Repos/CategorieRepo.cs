using Blog.Areas.Identity.Data;
using Blog.Data;
using Microsoft.EntityFrameworkCore;

namespace Blog.Repos
{
    public class CategorieRepo : ICategorieRepo
    {
        private readonly BlogContext _context;
        public CategorieRepo(BlogContext context) { _context = context; }
        public async Task<Categorie> Get(int id)
        {
            if (id == 0 || id == null) return null;
            var c = await _context.Categories.SingleAsync(c => c.Id == id);
            if(c == null) return null;
            return c;
        }
        public async Task<ICollection<Categorie>> GetAll()
        {
            return await _context.Categories
                .Include(p => p.PostCategories)
                .ToListAsync();
        }
    }
}
