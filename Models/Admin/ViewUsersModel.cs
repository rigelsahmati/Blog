using Blog.Areas.Identity.Data;

namespace Blog.Models.Admin
{
    public class ViewUsersModel
    {
        public string PageTitle { get; set; }
        public IEnumerable<User> Users { get; set; }
    }
}
