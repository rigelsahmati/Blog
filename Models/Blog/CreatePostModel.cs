using Blog.Areas.Identity.Data;
using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;

namespace Blog.Models.Blog
{
    public class CreatePostModel
    {
        [Required]
        public required string UserId { get; set; }

        [Required]
        [Display(Name = "Post Title: ", Description = "Enter a title for your post", Prompt = "Enter a title for your post.")]
        public string Title { get; set; }

        [Required]
        public string Context { get; set; }
        public IEnumerable<Categorie> Categories { get; set; }
    }
}
