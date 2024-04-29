using Blog.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace Blog.Data;

public class BlogContext : IdentityDbContext<User>
{
   // private readonly UserManager<User> _userManager;
    public BlogContext(DbContextOptions<BlogContext> options/*,UserManager<User> userManager*/): base(options)
    {
       // _userManager = userManager;
    }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Categorie> Categories { get; set; }
    public DbSet<PostCategorie> PostCategories { get; set; }
    public DbSet<Like> Likes { get; set; }
    public DbSet<Coment> Coments { get; set; }
    public DbSet<PostStatus> PostStatuses { get; set; }

    protected override async void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Role>()
           .HasMany(u => u.Users)
           .WithOne(r => r.Role)
           .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<User>()
           .HasMany(p => p.Posts)
           .WithOne(u => u.User)
           .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<User>()
           .HasMany(l => l.LikedPosts)
           .WithOne(u => u.User)
           .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<User>()
           .HasMany(c => c.Coments)
           .WithOne(u => u.User)
           .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<Post>()
            .HasMany(l => l.Likes)
            .WithOne(p => p.Post)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Post>()
            .HasMany(c => c.Coments)
            .WithOne(p => p.Post)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<PostStatus>()
            .HasMany(p => p.Posts)
            .WithOne(s => s.PostStatus)
            .OnDelete(DeleteBehavior.Restrict);

        //Categorie Seeds
        var categories = new string[] { "Nature", "Sport", "Politics", "Economy", "Art", "Science", "Entertainment", "News", "War", "Showbiz"};
        var icons = new string[] { "fa-mountain-sun", "fa-volleyball", "fa-scale-unbalanced-flip", "fa-landmark", "fa-palette", "fa-flask-vial", "fa-puzzle-piece", "fa-tv", "fa-person-rifle", "fa-camera-retro" };

         for (var i = 0; i < categories.Length; i++)
         {
             var c = new Categorie { Id = i +1 , Name= categories[i], Icon = icons[i]};
             builder.Entity<Categorie>().HasData(c);
         };

        var roles = new string[] { "Admin ", "Editor", "Member" };
        for(var i = 0;i < roles.Length; i++)
        {
            var r = new Role { id = i + 1, Name = roles[i], };
            builder.Entity<Role>().HasData(r);
        }
        var Ps = new string[] { "Active ", "Draft"};
        var PsIcon = new string[] { "fa-globe", "fa-envelope-open-text" };
        for(var i = 0;  i < Ps.Length; i++)
        {
            var ps = new PostStatus { Id = i + 1, Status = Ps[i], StatusIcon = PsIcon[i] };
            builder.Entity<PostStatus>().HasData(ps);
        }

        var hasher = new PasswordHasher<User>();
        var admin = new User {
            RoleId = 1,
            UserName = "admin",
            NormalizedUserName = "ADMIN",
            Email = "admin@blog.com",
            NormalizedEmail = "ADMIN@BLOG.COM",
            FullName = "Admin",
            CreatedTime = DateTime.Now,
            ProfileImage = "defaultUserImage.jpg",
            CoverImage = "defaultUserCover-3.jpg",
            LockoutEnabled = false,
        };
        admin.PasswordHash = hasher.HashPassword(admin, "admin1234");
        builder.Entity<User>().HasData(admin);

    }
}
