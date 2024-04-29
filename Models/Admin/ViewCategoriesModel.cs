using Blog.Areas.Identity.Data;

namespace Blog.Models.Admin
{
    public class ViewCategoriesModel
    {
        public string PageTitle { get; set; }
        public IEnumerable<Categorie> Categories { get; set; }
    }
}
