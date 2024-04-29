using Blog.Areas.Identity.Data;
using Blog.Data;
using Blog.Repos;
using Microsoft.EntityFrameworkCore;

namespace Blog.Repos
{
    public class UserRepo : IUserRepo
    {
        private readonly BlogContext _context;
        private readonly IPostRepo _postRepo;
        public UserRepo(BlogContext context, IPostRepo postRepo)
        {
            _context = context;
            _postRepo = postRepo;
        }
        public async Task<User> Get(string id)
        {
            var user = await _context.Users
                .Include(r => r.Role)
                .Include(l => l.LikedPosts).ThenInclude(p => p.Post).ThenInclude(c => c.Coments)
                .SingleOrDefaultAsync(x => x.Id == id);
            if (user == null) return null;
            
          //To Include Likes & Coments To post
            user.Posts = await _postRepo.GetUserPosts(user.Id);
            return user;
        }
        public async Task<IEnumerable<User>> GetAll()
        {
            return await _context.Users
                .Include(r => r.Role)
                .Include(p => p.Posts.Where(s => s.PostStatusId != 2))
                .ToListAsync();
        }
        public async Task<IEnumerable<User>> FilterByusername(string username, int? roleType)
        {
            var users = await GetAll();

            if (roleType == null)
            {
                var all = users
               .Where(x => x.UserName.ToLower().Contains(username.ToLower()))
               .ToList();
                return all;
            }
            var matchedUsers = users.Where(x => x.RoleId == roleType).Where(x => x.UserName.ToLower().Contains(username.ToLower())).ToList();

            return matchedUsers;
        }
    }
}
