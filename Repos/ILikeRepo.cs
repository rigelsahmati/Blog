using Blog.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace Blog.Repos
{
    public interface ILikeRepo
    {
        Task<Like> GetLike(int postId,string userId);
        Task<Post> GetPost(int postId);
        Task<bool> Like(string userId, int postId);
        
    }
}
