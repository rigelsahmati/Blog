using Blog.Areas.Identity.Data;

namespace Blog.Repos
{
    public interface ICategorieRepo
    {
        Task<Categorie> Get(int id);
        Task<ICollection<Categorie>> GetAll();
    }
}
