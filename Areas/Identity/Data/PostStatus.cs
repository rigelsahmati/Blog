using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blog.Areas.Identity.Data
{
    public class PostStatus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Status { get; set; }
        public string StatusIcon { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
    }
}
