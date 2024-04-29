using Microsoft.AspNetCore.Mvc;

namespace Blog.Repos
{
    public interface IPostCategorieRepo
    {
        Task CreatePostRepo(int postId, int categorieId);
    }
}
