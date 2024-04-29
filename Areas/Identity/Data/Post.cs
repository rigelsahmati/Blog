using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blog.Areas.Identity.Data
{
    public class Post
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public DateTime CreatedTime { get; set; }
        public string Title { get; set; }
        public string Context { get; set; }
        public string ImageUrl { get; set; }
        public string UserId { get; set; }
        public int PostStatusId { get; set; }

        [NotMapped]
        public bool Liked { get; set; } = false;

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }
        
        [ForeignKey(nameof(PostStatusId))] 
        public virtual PostStatus PostStatus { get; set; }

        public virtual ICollection<PostCategorie> PostCategories { get; set; }
        public virtual ICollection<Like> Likes { get; set; }
        public virtual IEnumerable<Coment> Coments { get; set; }

    }
}
