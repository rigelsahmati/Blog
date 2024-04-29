using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Blog.Areas.Identity.Data;

// Add profile data for application users by adding properties to the User class
public class User : IdentityUser
{
    public string FullName { get; set; }
    public int RoleId { get; set; }
    public DateTime CreatedTime { get; set; }
    public string ProfileImage { get; set; } = "defaultUserImage.png";
    public string CoverImage { get; set; }


    [ForeignKey(nameof(RoleId))]
    public virtual Role Role { get; set; }

    public virtual IEnumerable<Post> Posts { get; set; }
    public virtual ICollection<Like> LikedPosts { get; set; }
    public virtual IEnumerable<Coment> Coments { get; set; }

}

