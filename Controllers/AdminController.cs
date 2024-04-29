using Blog.Areas.Identity.Data;
using Blog.Models;
using Blog.Models.Admin;
using Blog.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Blog.Repos;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace Blog.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly IUserRepo _userRepo;
        private readonly ICategorieRepo _categorieRepo;
        private readonly IPostRepo _postRepo;
        private readonly IRoleRepo _roleRepo;
        private readonly SignInManager<User> _signInManager;
        private readonly IHostingEnvironment _host;


        public AdminController(
            ILogger<HomeController> logger,
            UserManager<User> userManager,
            IUserRepo userRepo,
            ICategorieRepo categorieRepo,
            IPostRepo postRepo,
            IRoleRepo roleRepo,
            SignInManager<User> signInManager,
            IHostingEnvironment host)
        {
            _logger = logger;
            _userManager = userManager;
            _userRepo = userRepo;
            _categorieRepo = categorieRepo;
            _postRepo = postRepo;
            _roleRepo = roleRepo;
            _signInManager = signInManager;
            _host = host;
        }

        #region ReturnViews
        public async Task<IActionResult> Index()
        {
            var acc = await _userManager.GetUserAsync(User);
            if (acc == null || acc.RoleId != 1) return RedirectToAction("Index", "Home");
            if(acc.LockoutEnabled) { await _signInManager.SignOutAsync(); return RedirectToPage("/Identity/Account/Lockout"); };
            var users = await _userRepo.GetAll();
            var model = new ViewUsersModel { Users = users, PageTitle = "Users" };
            return View(model);
        }
        public async Task<IActionResult> Admins()
        {
            var acc = await _userManager.GetUserAsync(User);
            if (acc == null || acc.RoleId != 1) return RedirectToAction("Index", "Home");
            if (acc.LockoutEnabled) { await _signInManager.SignOutAsync(); return RedirectToPage("/Identity/Account/Lockout"); };
            var users = await _userRepo.GetAll();
            var filterdUsers = users.Where(x => x.RoleId == 1);
            var model = new ViewUsersModel { Users = filterdUsers, PageTitle = "Admins" };
            return View(model);
        }
        public async Task<IActionResult> Editors()
        {
            var acc = await _userManager.GetUserAsync(User);
            if (acc == null ||  acc.RoleId != 1) return RedirectToAction("Index", "Home");
            if (acc.LockoutEnabled) { await _signInManager.SignOutAsync(); return RedirectToPage("/Identity/Account/Lockout"); };
            var users = await _userRepo.GetAll();
            var filterdUsers = users.Where(x => x.RoleId == 2);
            var model = new ViewUsersModel { Users = filterdUsers, PageTitle = "Editors" };
            return View( model);
        }
        public async Task<IActionResult> Members()
        {
            var acc = await _userManager.GetUserAsync(User);
            if (acc == null || acc.RoleId != 1) return RedirectToAction("Index", "Home");
            if (acc.LockoutEnabled) { await _signInManager.SignOutAsync(); return RedirectToPage("/Identity/Account/Lockout"); };
            var users = await _userRepo.GetAll();
            var filterdUsers = users.Where(x => x.RoleId == 3);
            var model = new ViewUsersModel { Users = filterdUsers, PageTitle = "Members" };
            return View(model);
        }
        public async Task<IActionResult> Categories()
        {
            var acc = await _userManager.GetUserAsync(User);
            if (acc == null || acc.RoleId != 1) return RedirectToAction("Index", "Home");
            if (acc.LockoutEnabled) { await _signInManager.SignOutAsync(); return RedirectToPage("/Identity/Account/Lockout"); };
            var model = new ViewCategoriesModel { PageTitle = "Categories",
                Categories = await _categorieRepo.GetAll(),
            };
            return View(model);
        }
        public async Task<IActionResult> Posts(int? categorieId)
        {
            var acc = await _userManager.GetUserAsync(User);
            if (acc == null || acc.RoleId != 1) return RedirectToAction("Index", "Home");
            if (acc.LockoutEnabled) { await _signInManager.SignOutAsync(); return RedirectToPage("/Identity/Account/Lockout"); };
            acc = await _userRepo.Get(acc.Id);
            var LikedPosts = await _postRepo.GetLikedPosts(acc.LikedPosts);
            var posts = await _postRepo.GetAll();
            foreach (var p in posts)
            {
                if (LikedPosts.Contains(p)) p.Liked = true;
                else p.Liked = false;
            }
            var model = new ViewPostsModel {
              PageTitle = "Posts",
              User = await _userRepo.Get(acc.Id),
              Posts = posts,
              Categories = await _categorieRepo.GetAll(),
            };
            if(categorieId != null) ViewBag.CategorieId = categorieId;
            return View(model);
        }
        public async Task<IActionResult> ViewUser(string? id,bool owns = false)
        {
            var admin = await _userManager.GetUserAsync(User);
            if (admin == null || admin.RoleId != 1) return RedirectToAction("Index", "Home");
            if (admin.LockoutEnabled) { await _signInManager.SignOutAsync(); return RedirectToPage("/Identity/Account/Lockout"); };
            admin = await _userRepo.Get(admin.Id);
            if ((bool)owns) id = admin.Id;
            if (string.IsNullOrWhiteSpace(id)) return View("NotFound");
            var acc = await _userRepo.Get(id);
            if (acc == null) return View("NotFound");
            var model = new ViewUserModel { 
                User = acc,
                Roles = await _roleRepo.GetAll(),
                ViewerLikedPosts = await _postRepo.GetLikedPosts(admin.LikedPosts),
                Categories = await _categorieRepo.GetAll(),
                AdminId = admin.Id,
                RoleId = acc.RoleId, 
            };
            return View(model);
        }
        #endregion

        #region AccountActions
        [HttpPost]
        public async Task<IActionResult> EditUser(string AdminId, int RoleId, string userId)
        {
            var acc = await _userManager.GetUserAsync(User);
            if (acc == null || acc.RoleId != 1) return RedirectToAction("Index", "Home");
            if (acc.LockoutEnabled) { await _signInManager.SignOutAsync(); return RedirectToPage("/Identity/Account/Lockout"); };
            if ( AdminId != acc.Id || RoleId < 1 || RoleId > 3 ) return RedirectToAction(nameof(ViewUser), new { id = userId});
            var toUpdate = await _userRepo.Get(userId);
            if (toUpdate == null) return RedirectToAction(nameof(ViewUser), new { id = userId });
            toUpdate.RoleId = RoleId;
            try{
              await _userManager.UpdateAsync(toUpdate);
            }catch (Exception ex)
            {
                TempData["Message"] = "Something Went Wrong!";
                TempData["Type"] = "Error";
                return RedirectToAction(nameof(ViewUser), new { id = userId });
            }
            if (acc.Id == userId)
            {
                return Redirect("~/");
            }
            TempData["Message"] = "Account Edited Sucessfuly!";
            TempData["Type"] = "Sucess";
            return RedirectToAction(nameof(ViewUser), new { id = userId });
        }

        [HttpPost]
        public async Task<IActionResult> BannUser(string userId, int bannTimeId)
        {
            var admin = await _userManager.GetUserAsync(User);
            if( admin == null || admin.RoleId != 1) return RedirectToAction("Index", "Home");
            if (admin.LockoutEnabled) { await _signInManager.SignOutAsync(); return RedirectToPage("/Identity/Account/Lockout"); };
            var user = await _userRepo.Get(userId);
            if( user == null) return RedirectToAction("Index", "Home");
            if( bannTimeId == null || bannTimeId > 6 || bannTimeId < 0) return RedirectToAction(nameof(ViewUser), new { id = userId });
            user.LockoutEnabled = true;
            switch (bannTimeId){
                case 0:
                    user.LockoutEnabled = false;
                    user.LockoutEnd = null;
                    break;
                case 1:
                    user.LockoutEnd = DateTime.Now.AddDays(1);
                    break;
                case 2:
                    user.LockoutEnd = DateTime.Now.AddDays(3);
                    break;
                case 3:
                    user.LockoutEnd = DateTime.Now.AddDays(7);
                    break;
                case 4:
                    user.LockoutEnd = DateTime.Now.AddMonths(1);
                    break;
                case 5:
                    user.LockoutEnd = DateTime.Now.AddMonths(3);
                    break;
                case 6:
                    user.LockoutEnd = DateTime.MaxValue;
                    break;
            }
            try
            {
                await _userManager.UpdateAsync(user);
            }catch(Exception ex)
            {
                TempData["Message"] = "Something Went Wrong!";
                TempData["Type"] = "Error!";
                return RedirectToAction(nameof(ViewUser), new { id = userId });
            }
            TempData["Message"] = "Account Banned Sucessfuly!";
            TempData["Type"] = "Sucess";
            return RedirectToAction(nameof(ViewUser), new { id = userId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string AdminId, string userId)
        {
            var acc = await _userManager.GetUserAsync(User);
            if (acc == null || acc.RoleId != 1) return RedirectToAction("Index", "Home");
            if (acc.LockoutEnabled) { await _signInManager.SignOutAsync(); return RedirectToPage("/Identity/Account/Lockout"); };
            if (AdminId != acc.Id) return RedirectToAction(nameof(ViewUser));
            var toDelet = await _userRepo.Get(userId);
            if (toDelet == null) return RedirectToAction(nameof(ViewUser));
            try
            {
                if (acc.ProfileImage != "defaultUserImage.jpg")
                {
                    var uploadFolderUsr = Path.Combine(_host.WebRootPath, "uploads/img/usersImg");
                    var existingFilePathUsr = Path.Combine(uploadFolderUsr, toDelet.ProfileImage);
                    FileInfo fileInfoUsr = new FileInfo(existingFilePathUsr);
                    if (fileInfoUsr.Exists) fileInfoUsr.Delete();
                }
                if (!acc.CoverImage.ToLower().Contains("defaultusercover-"))
                {
                    var uploadFolder = Path.Combine(_host.WebRootPath, "uploads/img/profileCover");
                    var existingFilePath = Path.Combine(uploadFolder, toDelet.CoverImage);
                    FileInfo fileInfo = new FileInfo(existingFilePath);
                    if (fileInfo.Exists) fileInfo.Delete();
                }
              
                await _userManager.DeleteAsync(toDelet);
            }
            catch(Exception ex) {
                TempData["Message"] = "Something Went Wrong!";
                TempData["Type"] = "Error";
                return RedirectToAction(nameof(ViewUser), new {id = userId});
            }
            if (acc.Id == userId)
            {
                await _signInManager.SignOutAsync();
                return Redirect("~/");
            }
            TempData["Message"] = "Account Deleted Sucessfuly!";
            TempData["Type"] = "Sucess";
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region AjaxCall
        [HttpGet]
        public async Task<JsonResult> GetByUsername(string username, int? roleType)
        {
            var acc = await _userManager.GetUserAsync(User);
            if (acc == null || acc.RoleId != 1) return Json(data: null);
            if (acc.LockoutEnabled) { await _signInManager.SignOutAsync(); return Json(data: null); };
            if (string.IsNullOrWhiteSpace(username)) return Json(data: null);
            var users = await _userRepo.FilterByusername(username, roleType);
            if (users == null) return Json(data: null);
            var usersList = new List<GetUserFromApiModel>{};
            foreach (var u in users)
            {
                var i = new GetUserFromApiModel
                {
                    Id = u.Id,
                    Image = u.ProfileImage,
                    FullName = u.FullName,
                    Email = u.Email,
                    Role = u.Role.Name,
                    Blogs = u.Posts.Count(),
                    DateJoined = u.CreatedTime.ToString("dd:MM:yyyy"),
                };
                usersList.Add(i);
            }
            return Json(data: usersList);
        }

        [HttpGet]
        public async Task<JsonResult> SearchPost(string query, int id)
        {
            var acc = await _userManager.GetUserAsync(User);
            if (acc == null) return Json(data: null);
            if (acc.LockoutEnabled) { await _signInManager.SignOutAsync(); return Json(data: null); };
            if (string.IsNullOrWhiteSpace(query) || query == null ) return Json(data: null);
            var posts = await _postRepo.Search(query.ToLower(), id);
            if (posts == null) return Json(data: null);
            var postList = new List<GetPostApiModel> { };
            var viewer = await _userRepo.Get(_userManager.GetUserId(User));
            var likedPosts = await _postRepo.GetLikedPosts(viewer.LikedPosts);
            foreach (var p in posts.OrderByDescending(d => d.CreatedTime))
            {
                var post = new GetPostApiModel
                {
                    postId = p.id,
                    userId = p.UserId,
                    imageUrl = p.ImageUrl,
                    profileImage = p.User.ProfileImage,
                    title = p.Title,
                    fullName = p.User.FullName,
                    createdTime = p.CreatedTime.ToString("HH:mm dd/MM/yyyy"),
                    likes = p.Likes.Count,
                    coments = p.Coments.Count(),
                    viewerId = viewer.Id,
                    liked = false
                };
                if (likedPosts.Contains(p)) post.liked = true;
                postList.Add(post);
            }
            return Json(data: postList);
        }

        [HttpGet]
        public async Task<JsonResult> FilterPost(int id)
        {
            var acc = await _userManager.GetUserAsync(User);
            if (acc == null) return Json(data: null);
            if (acc.LockoutEnabled) { await _signInManager.SignOutAsync(); return Json(data: null); };
            if (id == 0) return Json(data: null);
            var posts = await _postRepo.Filter(id);
            if (posts == null) return Json(data: null);
            var postList = new List<GetPostApiModel> { };
            var viewer = await _userRepo.Get(_userManager.GetUserId(User));
            var likedPost = await _postRepo.GetLikedPosts(viewer.LikedPosts);
            foreach (var p in posts.OrderByDescending(d => d.CreatedTime))
            {
                var post = new GetPostApiModel
                {
                    postId = p.id,
                    userId = p.UserId,
                    imageUrl = p.ImageUrl,
                    profileImage = p.User.ProfileImage,
                    title = p.Title,
                    fullName = p.User.FullName,
                    createdTime = p.CreatedTime.ToString("HH:mm dd/MM/yyyy"),
                    likes = p.Likes.Count,
                    coments = p.Coments.Count(),
                    viewerId = viewer.Id,
                    liked = false
                };
                if (likedPost.Contains(p)) post.liked = true;
                postList.Add(post);
            }
            return Json(data: postList);
        }

        [HttpGet]
        public async Task<JsonResult> GetAllPosts()
        {
            var acc = await _userManager.GetUserAsync(User);
            if (acc == null) return Json(data: null);
            if (acc.LockoutEnabled) { await _signInManager.SignOutAsync(); return Json(data: null); };
            var posts = await _postRepo.GetAll();
            if (posts == null) return Json(data: null);
            var postList = new List<GetPostApiModel> { };
            var viewer = await _userRepo.Get(_userManager.GetUserId(User));
            var likedPost = await _postRepo.GetLikedPosts(viewer.LikedPosts);
            foreach (var p in posts.OrderByDescending(d => d.CreatedTime))
            {
                var post = new GetPostApiModel
                {
                    postId = p.id,
                    userId = p.UserId,
                    imageUrl = p.ImageUrl,
                    profileImage = p.User.ProfileImage,
                    title = p.Title,
                    fullName = p.User.FullName,
                    createdTime = p.CreatedTime.ToString("HH:mm dd/MM/yyyy"),
                    likes = p.Likes.Count,
                    coments = p.Coments.Count(),
                    viewerId = viewer.Id,
                    liked = false
                };
                if (likedPost.Contains(p)) post.liked = true;
                postList.Add(post);
            }
            return Json(data: postList);
        }

        #endregion

    }
}
