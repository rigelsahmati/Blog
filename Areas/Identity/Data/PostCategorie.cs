using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blog.Areas.Identity.Data
{

    [PrimaryKey(nameof(PostId),nameof(CategorieId))]
    public class PostCategorie
    {
        public int PostId { get; set; }
        public int CategorieId { get; set; }


        [ForeignKey(nameof(PostId))]
        public virtual Post Post { get; set; }

        [ForeignKey(nameof(CategorieId))]
        public virtual Categorie Categorie { get; set; }
    }
}
