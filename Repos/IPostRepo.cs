using Blog.Areas.Identity.Data;
namespace Blog.Repos
{
    public interface IPostRepo
    {
        Task Create(Post post, List<int> postCategories);
        Task Update(Post post, List<int> postCategories);
        Task Delete(Post post);
        Task<Post> Get(int id);
        Task<IEnumerable<Post>> LoadPosts(string time);
        Task<IEnumerable<Post>> LatestPosts(int postId);
        Task<IEnumerable<Post>> SimilarPosts(IEnumerable<PostCategorie> categories, int postId);
        Task<IEnumerable<Post>> GetLikedPosts(ICollection<Like> likes);
        Task<IEnumerable<Post>> GetUserPosts(string userId);
        Task<IEnumerable<Post>> GetUserDrafts(string userId);
        Task<int> GetLikeCount(int postId);
        Task<IEnumerable<Post>> FilterUserPosts(string userId, int? categorieId, string? fromTime, string? toTime);
        Task<IEnumerable<Post>> GetAll();
        Task<IEnumerable<Post>> Search(string query, int id);
        Task<IEnumerable<Post>> Filter(int id);
        Task<IEnumerable<Coment>> GetComents(int id);
        Task<bool> AddComent(int postid, string userId, string comentContext);

    }
}
