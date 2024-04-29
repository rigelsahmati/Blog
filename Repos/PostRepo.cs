using Blog.Areas.Identity.Data;
using Blog.Data;
using Microsoft.EntityFrameworkCore;

namespace Blog.Repos
{
    public class PostRepo : IPostRepo
    {
        private readonly BlogContext _context;
        private readonly IPostCategorieRepo _postCategorieRepo;
        public PostRepo(BlogContext context, IPostCategorieRepo postCategorieRepo)
        {
            _context = context;
            _postCategorieRepo = postCategorieRepo;
        }

        public async Task Create(Post post,List<int> postCategories)
        {
            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();
            foreach (var pc in postCategories) {
                await _postCategorieRepo.CreatePostRepo(post.id ,pc);
            };
        }
        public async Task Update(Post post, List<int> postCategories)
        {
             _context.Update(post);
            await _context.SaveChangesAsync();
            if (postCategories != null)
            {
                foreach (var pc in post.PostCategories)
                {
                    _context.Remove(pc);
                    await _context.SaveChangesAsync();
                }
                foreach (var pc in postCategories)
                {
                    await _postCategorieRepo.CreatePostRepo(post.id, pc);
                };
            }
            
        }
        public async Task Delete(Post post)
        {
            _context.Remove(post);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<Post>> GetAll()
        {
            var data = await _context.Posts
                .Include(x => x.User)
                .Include(x => x.PostCategories)
                .Include(l => l.Likes)
                .Include(c => c.Coments)
                .OrderByDescending(t => t.CreatedTime)
                .Where(s => s.PostStatusId != 2)
                .ToListAsync();
            return data;
        }
        public async Task<IEnumerable<Post>> LoadPosts(string time)
        {
            var postTime = DateTime.Parse(time);
            var posts = await _context.Posts
                .Include(x => x.User)
                .Include(x => x.PostCategories)
                .Include(l => l.Likes)
                .Include(c => c.Coments)
                .OrderByDescending(t => t.CreatedTime)
                .Where(x => x.CreatedTime < postTime && x.PostStatusId != 2)
                .ToListAsync();
            List<Post> data = new();
            //data = (List<Post>)posts.Take(9);
            foreach (var p in posts)
            {
                if (data.Count == 9) break;
                data.Add(p);
            }
            return data;
        }
        public async Task<IEnumerable<Post>> GetUserPosts(string userId)
        {
            var posts = await _context.Posts
                .Include(u => u.User)
                .Include(l => l.Likes)
                .Include(p => p.PostCategories)
                .Include(c => c.Coments)
                .OrderByDescending(t => t.CreatedTime)
                .Where(x => x.UserId == userId && x.PostStatusId != 2).ToListAsync();
            return posts;
        }
        public async Task<IEnumerable<Post>> GetUserDrafts(string userId)
        {
            var posts = await _context.Posts
                .Include(u => u.User)
                .Include(l => l.Likes)
                .Include(p => p.PostCategories)
                .Include(c => c.Coments)
                .OrderByDescending(t => t.CreatedTime)
                .Where(x => x.UserId == userId && x.PostStatusId == 2).ToListAsync();
            return posts;
        }
        public async Task<IEnumerable<Post>> FilterUserPosts(string userId, int? categorieId, string? fromTime, string? toTime)
        {
            var posts = await GetUserPosts(userId);
            if (posts == null) return null;

            //Only FilterBy Categorie
            if (string.IsNullOrWhiteSpace(fromTime) && string.IsNullOrWhiteSpace(toTime))
            {
                var Categorie = await _context.Categories.SingleOrDefaultAsync(x => x.Id == categorieId);
                if (Categorie == null) return null;

                // posts = posts.Where(x => x.PostCategories.Contains(postCategorie));
                var result = posts.Where(p => p.PostCategories.Any(c => c.CategorieId == categorieId));
                return result;
            }

            //Only FilterBy From
            if (categorieId == 0 & string.IsNullOrWhiteSpace(toTime))
            {
                var dateFrom = DateTime.ParseExact(fromTime, "dd/MM/yyyy", null);
                posts = posts.Where(t => t.CreatedTime >= dateFrom);
                return posts;
            }

            //Only FilterBy To
            if (categorieId == 0 & string.IsNullOrWhiteSpace(fromTime))
            {
                var dateTo = DateTime.ParseExact(toTime, "dd/MM/yyyy", null);
                posts = posts.Where(t => t.CreatedTime.Date <= dateTo);
                return posts;
            }

            //FilterBy Categorie & From
            if (categorieId != 0 & !string.IsNullOrWhiteSpace(fromTime) & string.IsNullOrWhiteSpace(toTime))
            {
                var dateFrom = DateTime.ParseExact(fromTime, "dd/MM/yyyy", null);

                var Categorie = await _context.Categories.SingleOrDefaultAsync(x => x.Id == categorieId);
                if (Categorie == null) return null;

                posts = posts.Where(x => x.PostCategories.Any(c => c.CategorieId == categorieId) && x.CreatedTime >= dateFrom).ToList();
                return posts;
            }

            //FilterBy Categorie & To
            if (categorieId != 0 & string.IsNullOrWhiteSpace(fromTime) & !string.IsNullOrWhiteSpace(toTime))
            {
                var dateTo = DateTime.ParseExact(toTime, "dd/MM/yyyy", null);

                var Categorie = await _context.Categories.SingleOrDefaultAsync(x => x.Id == categorieId);
                if (Categorie == null) return null;

                posts = posts.Where(x => x.PostCategories.Any(c => c.CategorieId == categorieId) && x.CreatedTime.Date <= dateTo).ToList();
                return posts;
            }

            //FilterBy From & To
            if (categorieId == 0 & !string.IsNullOrWhiteSpace(fromTime) && !string.IsNullOrWhiteSpace(toTime))
            {
                var dateFrom = DateTime.ParseExact(fromTime, "dd/MM/yyyy", null);
                var dateTo = DateTime.ParseExact(toTime, "dd/MM/yyyy", null);
                posts = posts.Where(x => x.CreatedTime.Date <= dateTo && x.CreatedTime >= dateFrom);
                return posts;
            }

            //FilterBy All
            if (categorieId != 0 & !string.IsNullOrWhiteSpace(fromTime) & !string.IsNullOrWhiteSpace(toTime))
            {
                var dateFrom = DateTime.ParseExact(fromTime, "dd/MM/yyyy", null);
                var dateTo = DateTime.ParseExact(toTime, "dd/MM/yyyy", null);

                var Categorie = await _context.Categories.SingleOrDefaultAsync(x => x.Id == categorieId);
                if (Categorie == null) return null;

                posts = posts.Where(x => x.PostCategories.Any(c => c.CategorieId == categorieId) && x.CreatedTime.Date <= dateTo && x.CreatedTime >= dateFrom);
                return posts;
            }

            return null;
        }
        public async Task<IEnumerable<Post>> Filter(int id)
        {
            var Categorie = await _context.Categories.SingleOrDefaultAsync(x => x.Id == id);
            if (Categorie == null) return null;

            var posts = await GetAll();
            var machedPosts = posts.Where(c => c.PostCategories.Any(x => x.CategorieId == id)).ToList();
            return machedPosts;
        }
        public async Task<IEnumerable<Post>> Search(string query, int id)
        {
            if (id == 0)
            {
                var posts = await GetAll();
                var matchQuery = posts.Where(t => t.Title.ToLower().Contains(query) || t.User.FullName.ToLower().Contains(query))
                    .ToList()
                    .OrderByDescending(t => t.CreatedTime);
                return matchQuery;
            }

            var filterdPosts = await Filter(id);
            if (filterdPosts == null) return null;
          //  var categories = await _context.Categories.Where(x => x.Name.ToLower().Contains(query.ToLower())).ToListAsync();
            var matchQueryFilter = filterdPosts.Where(t => t.Title.ToLower().Contains(query) || t.User.FullName.ToLower().Contains(query))
                    .ToList()
                    .OrderByDescending(t => t.CreatedTime);
            return matchQueryFilter;
        }
        public async Task<Post> Get(int id)
        {
            var post = await _context.Posts
                .Include(u => u.User)
                .Include(l => l.Likes)
                .Include(c => c.Coments).ThenInclude(u => u.User)
                .Include(x => x.PostCategories)
                .SingleOrDefaultAsync(x => x.id == id);
            if(post == null) return null;
            return post;
        }
        public async Task<IEnumerable<Post>> GetLikedPosts(ICollection<Like> likes)
        {
            if (likes == null) return null;
            List<Post> posts = new();

            foreach (var l in likes)
            {
                var p = await Get((int)l.PostId);
                if (p != null || p.PostStatusId != 1 ) posts.Add(p);

            }
            return posts;
        }
       
        public async Task<int> GetLikeCount(int postId)
        {
            var post = await _context.Posts.Include(l => l.Likes).SingleOrDefaultAsync(x => x.id == postId);
            if (post == null) return -1;
            return post.Likes.Count();



        }
        public async Task<IEnumerable<Post>> LatestPosts(int postId)
        {
            var posts = await _context.Posts
                .Include(u => u.User)
                .OrderByDescending(post => post.CreatedTime)
                .Where(s => s.PostStatusId != 2 && s.id != postId)
                .Take(3)
                .ToListAsync();
            if (posts == null) return null;
            return posts;
        }
        public async Task<IEnumerable<Post>> SimilarPosts(IEnumerable<PostCategorie> categories, int postId)
        {
            if (categories == null) return null;

            List<Post> posts = new();
            foreach (var cat in categories)
            {
                var dbPosts = await _context.Posts
                  .Include(u => u.User)
                  .Include(x => x.PostCategories)
                  .OrderByDescending(post => post.CreatedTime)
                  .Where(s => s.PostStatusId != 2 && s.id != postId && s.PostCategories.Any(x => x.CategorieId == cat.CategorieId))
                  .Take(3)
                  .ToListAsync();

                if (dbPosts.Count == 3)
                {
                    foreach (var p in dbPosts)
                    {
                        if (posts.Count == 3) break;
                        if (posts.Contains(p)) continue;
                        posts.Add(p);
                        break;
                    }
                }
                else
                {
                    foreach (var p in dbPosts)
                    {
                        if (posts.Count == 3) break;
                        if (posts.Contains(p)) continue;
                        posts.Add(p);
                    }
                }

            }

            if (posts == null) return null;
            return posts;
        }
        public async Task<IEnumerable<Coment>> GetComents(int id)
        {
            var post = await Get(id);
            if (post == null) return null;
            var coments = post.Coments.OrderByDescending(t => t.CreatedTime).ToList();
            return coments;
        }
        public async Task<bool> AddComent(int postid, string userId, string comentContext)
        {
            var coment = new Coment { ComentContext = comentContext, CreatedTime = DateTime.Now, PostId = postid, UserId = userId };
            try
            {
              await _context.Coments.AddAsync(coment);
              await _context.SaveChangesAsync();
              return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            
        }
    }
}
