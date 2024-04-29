using Blog.Areas.Identity.Data;

namespace Blog.Repos
{
    public interface IRoleRepo
    {
        Task<Role> Get(int id);
        Task<IEnumerable<Role>> GetAll();
    }
}
