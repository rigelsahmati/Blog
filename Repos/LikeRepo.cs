using Blog.Areas.Identity.Data;
using Blog.Data;
using Microsoft.EntityFrameworkCore;
namespace Blog.Repos
{
    public class LikeRepo : ILikeRepo
    {
        private readonly BlogContext _context;
        public LikeRepo(BlogContext context)
        {
            _context = context;
        }
        public async Task<Post> GetPost(int postId)
        {
            var post= await _context.Posts.Include(l => l.Likes).Where(s => s.PostStatusId == 1).SingleOrDefaultAsync(x => x.id == postId);
            if (post == null) return null;
            return post;
        }
        public async Task<Like> GetLike(int postId, string userId)
        {
            var like = await _context.Likes.SingleOrDefaultAsync(x => x.UserId == userId && x.PostId == postId);
            if (like == null) return null;
            return like;
        }
        public async Task<bool> Like(string userId, int postId)
        {
            var like = await GetLike(postId , userId);
            var post = await GetPost(postId);
            if (post == null) return false;
            if (like == null)
            {
                var newLike = new Like { UserId = userId, PostId = postId };
                try
                {
                    await _context.Likes.AddAsync(newLike);
                }catch(Exception ex)
                {
                    return false;
                }
                await _context.SaveChangesAsync();
                return true;
            }
            if (post.Likes.Contains(like))
            {
                try
                {
                 _context.Likes.Remove(like);
                }catch(Exception ex)
                {
                    return false;
                }
                await _context.SaveChangesAsync();
                return true;
            }

            return false;

        }
    }
}
