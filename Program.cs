using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Blog.Data;
using Blog.Areas.Identity.Data;
using Blog.Repos;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("BlogContextConnection") ?? throw new InvalidOperationException("Connection string 'BlogContextConnection' not found.");

builder.Services.AddDbContext<BlogContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<User>(options => 
{ 
    options.SignIn.RequireConfirmedAccount = false; 
    options.Lockout.AllowedForNewUsers = false; }
)
.AddEntityFrameworkStores<BlogContext>();
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
});

builder.Services.AddScoped<IUserRepo, UserRepo>();
builder.Services.AddScoped<ICategorieRepo, CategorieRepo>();
builder.Services.AddScoped<IPostRepo, PostRepo>();
builder.Services.AddScoped<IRoleRepo, RoleRepo>();
builder.Services.AddScoped<ILikeRepo, LikeRepo>();
builder.Services.AddScoped<IPostCategorieRepo, PostCategorieRepo>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
app.Run();
