using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blog.Areas.Identity.Data
{
    public class Coment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime CreatedTime { get; set; }
        [ForeignKey(nameof(User))] public string UserId { get; set; }
        [ForeignKey(nameof(Post))] public int PostId { get; set; }
        public string ComentContext { get; set; }
        
        public virtual User User { get; set; }
        public virtual Post Post { get; set; }
    }
}
