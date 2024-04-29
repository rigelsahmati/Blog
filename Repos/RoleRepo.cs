using Blog.Areas.Identity.Data;
using Blog.Data;
using Microsoft.EntityFrameworkCore;

namespace Blog.Repos
{
    public class RoleRepo : IRoleRepo
    {
        private readonly BlogContext _context;
        public RoleRepo(BlogContext context)
        {
            _context = context;
        }
        public async Task<Role> Get(int id)
        {
            if (id == 0 || id == null) return null;
            var r = await _context.Roles.SingleOrDefaultAsync(x => x.id == id);
            if (r == null) return null;
            return r;
        }
        public async Task<IEnumerable<Role>> GetAll()
        {
            var roles = await _context.Roles.ToListAsync();
            return roles;
        }
    }
}
