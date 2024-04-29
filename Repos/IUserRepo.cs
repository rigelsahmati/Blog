using Blog.Areas.Identity.Data;
namespace Blog.Repos
{
    public interface IUserRepo
    {
        Task<User> Get(string id);
        Task<IEnumerable<User>> GetAll();
        Task<IEnumerable<User>> FilterByusername(string username, int? roleType);
    }
}
