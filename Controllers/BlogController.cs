using Blog.Areas.Identity.Data;
using Blog.Models;
using Blog.Models.Blog;
using Blog.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace Blog.Controllers
{
    [Authorize]
    public class BlogController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IUserRepo _userRepo;
        private readonly ICategorieRepo _categorieRepo;
        private readonly IPostRepo _postRepo;
        private readonly IRoleRepo _roleRepo;
        private readonly SignInManager<User> _signInManager;
        private readonly IHostingEnvironment _host;
        private readonly IPostCategorieRepo _postCategorieRepo;


        public BlogController(
            UserManager<User> userManager,
            IUserRepo userRepo,
            ICategorieRepo categorieRepo,
            IPostRepo postRepo,
            IRoleRepo roleRepo,
            SignInManager<User> signInManager,
            IHostingEnvironment host,
            IPostCategorieRepo postCategorieRepo)
        {
            _userManager = userManager;
            _userRepo = userRepo;
            _categorieRepo = categorieRepo;
            _postRepo = postRepo;
            _roleRepo = roleRepo;
            _signInManager = signInManager;
            _host = host;
            _postCategorieRepo = postCategorieRepo;

        }

        #region Return Views
        public async Task<IActionResult> Index()
        {
            var acc = await _userManager.GetUserAsync(User);
            if (acc == null || acc.RoleId == 1) return RedirectToAction("Index", "Home");
            if (acc.LockoutEnabled) { await _signInManager.SignOutAsync(); return RedirectToPage("/Identity/Account/Lockout"); };
            acc = await _userRepo.Get(acc.Id);
            var Posts = await _postRepo.LoadPosts(DateTime.Now.ToString());
            var LikedPosts = await _postRepo.GetLikedPosts(acc.LikedPosts);
            foreach (var p in Posts)
            {
                if (LikedPosts.Contains(p)) p.Liked = true;
                else p.Liked = false;
            }
            var model = new PostsIndexModel
            {
                PageTitle = "Home",
                Posts = Posts,
                User = acc,
                Categories = await _categorieRepo.GetAll(),
            };
            return View(model);
        }
        public async Task<IActionResult> Profile(string? id, bool owns = false)
        {
            var viewer = await _userManager.GetUserAsync(User);
            if (viewer == null) return RedirectToAction("Index", "Home");
            if (viewer.LockoutEnabled) { await _signInManager.SignOutAsync(); return RedirectToPage("/Identity/Account/Lockout"); };
            if (viewer.RoleId == 1) return RedirectToAction("ViewUser", "Admin", new { id = id });
            viewer = await _userRepo.Get(viewer.Id);
            if ((bool)owns) id = viewer.Id;
            if (string.IsNullOrWhiteSpace(id)) return View("NotFound");
            var acc = await _userRepo.Get(id);
            if (acc == null) return View("NotFound");
            var model = new ProfileModel
            {
                User = acc,
                LikedPosts = await _postRepo.GetLikedPosts(viewer.LikedPosts),
                Categories = await _categorieRepo.GetAll()
            };
            return View(model);
        }
        public async Task<IActionResult> MyPosts()
        {
            var acc = await _userManager.GetUserAsync(User);
            if (acc == null || acc.RoleId != 2) return RedirectToAction("Index", "Home");
            if (acc.LockoutEnabled) { await _signInManager.SignOutAsync(); return RedirectToPage("/Identity/Account/Lockout"); };
            acc = await _userRepo.Get(acc.Id);
            var Posts = await _postRepo.GetUserPosts(acc.Id);
            var LikedPosts = await _postRepo.GetLikedPosts(acc.LikedPosts);
            foreach (var p in Posts)
            {
                if (LikedPosts.Contains(p)) p.Liked = true;
            }
            var model = new PostsIndexModel
            {
                PageTitle = "My Posts",
                Posts = Posts,
                User = acc,
                Categories = await _categorieRepo.GetAll(),
            };
            return View(model);
        }
        public async Task<IActionResult> Drafts()
        {
            var acc = await _userManager.GetUserAsync(User);
            if (acc == null || acc.RoleId != 2) return RedirectToAction("Index", "Home");
            if (acc.LockoutEnabled) { await _signInManager.SignOutAsync(); return RedirectToPage("/Identity/Account/Lockout"); };
            acc = await _userRepo.Get(acc.Id);
            var Posts = await _postRepo.GetUserDrafts(acc.Id);
            var LikedPosts = await _postRepo.GetLikedPosts(acc.LikedPosts);
            foreach (var p in Posts)
            {
                if (LikedPosts.Contains(p)) p.Liked = true;
            }
            var model = new PostsIndexModel
            {
                PageTitle = "Draft Posts",
                Posts = Posts,
                User = acc,
                Categories = await _categorieRepo.GetAll(),
            };
            return View(model);
        }
        public async Task<IActionResult> LikedPosts()
        {
            var acc = await _userManager.GetUserAsync(User);
            if (acc == null || acc.RoleId == 1) return RedirectToAction("Index", "Home");
            if (acc.LockoutEnabled) { await _signInManager.SignOutAsync(); return RedirectToPage("/Identity/Account/Lockout"); };
            acc = await _userRepo.Get(acc.Id);
            var Posts = await _postRepo.GetUserPosts(acc.Id);
            var LikedPosts = await _postRepo.GetLikedPosts(acc.LikedPosts);
            foreach (var p in Posts)
            {
                if (LikedPosts.Contains(p)) p.Liked = true;
            }
            var model = new PostsIndexModel
            {
                PageTitle = "Liked Posts",
                Posts = LikedPosts,
                User = acc,
                Categories = await _categorieRepo.GetAll(),
            };
            return View(model);
        }
        public async Task<IActionResult> Post(int id)
        {
            var acc = await _userManager.GetUserAsync(User);
            if (acc == null) return RedirectToAction("Index", "Home");
            if (acc.LockoutEnabled) { await _signInManager.SignOutAsync(); return RedirectToPage("/Identity/Account/Lockout"); };
            acc = await _userRepo.Get(acc.Id);
            var post = await _postRepo.Get(id);
            if (post == null) return View("NotFound");
            if(post.PostStatusId != 1)
            {
                if(post.UserId != acc.Id) return View("NotFound");
            }
            var likedPosts = await _postRepo.GetLikedPosts(acc.LikedPosts);
            if (likedPosts.Contains(post)) post.Liked = true;
            var model = new ViewPostModel
            {
                Post = post,
                LatestPosts = await _postRepo.LatestPosts(post.id),
                SimilarPosts = await _postRepo.SimilarPosts(post.PostCategories, post.id)
            };
            return View(model);
        }
        public async Task<IActionResult> Posts(int categorieId)
        {
            var acc = await _userManager.GetUserAsync(User);
            if (acc == null || acc.RoleId == 1) return RedirectToAction("Index", "Home");
            if (acc.LockoutEnabled) { await _signInManager.SignOutAsync(); return RedirectToPage("/Identity/Account/Lockout"); };
            var categorie = await _categorieRepo.Get(categorieId);
            if(categorie == null) return RedirectToAction("Index", "Home");
            acc = await _userRepo.Get(acc.Id);
            var Posts = await _postRepo.Filter(categorieId);
            var LikedPosts = await _postRepo.GetLikedPosts(acc.LikedPosts);
            foreach (var p in Posts)
            {
                if (LikedPosts.Contains(p)) p.Liked = true;
                else p.Liked = false;
            }
            var model = new PostsIndexModel
            {
                PageTitle = categorie.Name,
                Posts = Posts,
                User = acc,
                Categories = await _categorieRepo.GetAll(),
            };
            ViewBag.CatIcon = categorie.Icon;
            return View(model);
        }
        public async Task<IActionResult> CreatePost()
        {
            var acc = await _userManager.GetUserAsync(User);
            if (acc == null || acc.RoleId != 2) return RedirectToAction("Index", "Home");
            if (acc.LockoutEnabled) { await _signInManager.SignOutAsync(); return RedirectToPage("/Identity/Account/Lockout"); };
            var model = new CreatePostModel { UserId = acc.Id, Categories = await _categorieRepo.GetAll() };
            return View(model);

        }
        public async Task<IActionResult> EditPost(int postId)
        {
            if (postId == 0 || postId == null) return RedirectToAction("Index", "Home");
            var acc = await _userManager.GetUserAsync(User);
            if (acc == null || acc.RoleId != 2) return RedirectToAction("Index", "Home");
            if (acc.LockoutEnabled) { await _signInManager.SignOutAsync(); return RedirectToPage("/Identity/Account/Lockout"); };
            var post = await _postRepo.Get(postId);
            if (post == null || post.UserId != acc.Id) return View("NotFound");
            var model = new EditPostModel { UserId = acc.Id, Categories = await _categorieRepo.GetAll(), Context = post.Context, Title = post.Title, ImageUrl = post.ImageUrl, PostId = post.id };
            List<int> categorieIds = new();
            foreach (var i in post.PostCategories) categorieIds.Add(i.CategorieId);
            ViewBag.Categories = categorieIds;
            return View(model);
        }
        #endregion

        #region Blog Action
        [HttpPost]
        public async Task<IActionResult> CreatePost(string UserId, string Title, string Context, List<int> categorieIds)
        {
            var acc = await _userManager.GetUserAsync(User);
            if (acc == null || acc.RoleId != 2 || acc.Id != UserId) return RedirectToAction("Index", "Home");
            if (acc.LockoutEnabled) { await _signInManager.SignOutAsync(); return RedirectToPage("/Identity/Account/Lockout"); };
            if (!ModelState.IsValid || categorieIds.Count() == 0 || HttpContext.Request.Form.Files.Count() == 0)
            {
                if (categorieIds == null || categorieIds.Count() == 0) TempData["ErrorMessage"] = "Please select at least 1 categorie!";
                else ViewBag.Categories = categorieIds;
                var model = new CreatePostModel { UserId = acc.Id, Categories = await _categorieRepo.GetAll(), Title = Title, Context = Context };
                TempData["Message"] = "Please fill all fields!";
                TempData["Type"] = "Error";
                return View(model);
            }
            var file = HttpContext.Request.Form.Files.First();
            var newFileName = $"{acc.Id}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}"; // New file name with acc.Id
            var uploadFolder = Path.Combine(_host.WebRootPath, "uploads/img/postImg");
            var addr = Path.Combine(uploadFolder, newFileName);
            using (var fileStream = new FileStream(addr, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            Post post = new()
            {
                Context = Context,
                CreatedTime = DateTime.Now,
                UserId = acc.Id,
                PostStatusId = 1,
                Title = Title,
                ImageUrl = newFileName, // Store the new file name in ImageUrl
            };
            await _postRepo.Create(post, categorieIds);
            return RedirectToAction(nameof(Post), new { id = post.id });
        }

        [HttpPost]
        public async Task<IActionResult> EditPost(string UserId, string Title, string Context, List<int> categorieIds, int postId)
        {
            var acc = await _userManager.GetUserAsync(User);
            if (acc == null || acc.RoleId != 2 || acc.Id != UserId) return RedirectToAction("Index", "Home");
            if (acc.LockoutEnabled) { await _signInManager.SignOutAsync(); return RedirectToPage("/Identity/Account/Lockout"); };
            var post = await _postRepo.Get(postId);
            if (post == null) return RedirectToAction("Index", "Home");
            if (!ModelState.IsValid || categorieIds.Count() == 0)
            {
                if (categorieIds == null || categorieIds.Count() == 0) TempData["ErrorMessage"] = "Please select at least 1 categorie!";
                else ViewBag.Categories = categorieIds;
                var model = new EditPostModel { UserId = acc.Id, Categories = await _categorieRepo.GetAll(), Context = Context, Title = Title, ImageUrl = post.ImageUrl };
                TempData["Message"] = "Please fill all fields!";
                TempData["Type"] = "Error";
                return View(model);
            }
            if (HttpContext.Request.Form.Files.Count() != 0)
            {
                var file = HttpContext.Request.Form.Files.First();
                var newFileName = $"{acc.Id}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var uploadFolder = Path.Combine(_host.WebRootPath, "uploads/img/postImg");
                // Remove existing image
                var existingFileName = Path.GetFileName(post.ImageUrl);
                var existingFilePath = Path.Combine(uploadFolder, existingFileName);
                FileInfo fileInfo = new FileInfo(existingFilePath);
                if (fileInfo.Exists)
                {
                    fileInfo.Delete();
                }
                var addr = Path.Combine(uploadFolder, newFileName);
                using (var fileStream = new FileStream(addr, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
                post.ImageUrl = newFileName;
            }
            post.Title = Title;
            post.Context = Context;
            try
            {
                await _postRepo.Update(post, categorieIds);
            } catch (Exception ex)
            {
                var model = new EditPostModel { UserId = acc.Id, Categories = await _categorieRepo.GetAll(), Context = Context, Title = Title, ImageUrl = post.ImageUrl };
                TempData["Message"] = "Something wrong happened!";
                TempData["Type"] = "Error";
                return View(model);
            }
            return RedirectToAction(nameof(Post), new { id = post.id });
        }

        [HttpPost]
        public async Task<IActionResult> DeletePost(string userId, int postId)
        {
            var acc = await _userManager.GetUserAsync(User);
            if (acc == null || acc.RoleId != 2 || acc.Id != userId) return RedirectToAction("Index", "Home");
            if (acc.LockoutEnabled) { await _signInManager.SignOutAsync(); return RedirectToPage("/Identity/Account/Lockout"); };
            acc = await _userRepo.Get(acc.Id);
            var post = await _postRepo.Get(postId);
            if (post == null) return RedirectToAction(nameof(Index));
            if (!acc.Posts.Contains(post)) return RedirectToAction(nameof(Index));

            try
            {
                // Remove existing image
                var uploadFolder = Path.Combine(_host.WebRootPath, "uploads/img/postImg");
                var existingFilePath = Path.Combine(uploadFolder, post.ImageUrl);
                FileInfo fileInfo = new FileInfo(existingFilePath);
                if (fileInfo.Exists) fileInfo.Delete();
                await _postRepo.Delete(post);
                TempData["Message"] = "Post deleted sucessfuly!";
                TempData["Type"] = "Sucess";
                return RedirectToAction(nameof(Index));
            }catch(Exception ex)
            {
                TempData["Message"] = "Something wrong happened!";
                TempData["Type"] = "Error";
                return RedirectToAction(nameof(EditPost), new { postId = postId });
            }
        }
        [HttpPost]
        public async Task<IActionResult> Search(string q)
        {
            var acc = await _userManager.GetUserAsync(User);
            if (acc == null) return RedirectToAction("Index", "Home");
            if (acc.LockoutEnabled) { await _signInManager.SignOutAsync(); return RedirectToPage("/Identity/Account/Lockout"); };
            acc = await _userRepo.Get(acc.Id);
            var posts = await _postRepo.Search(q, 0);
            var LikedPosts = await _postRepo.GetLikedPosts(acc.LikedPosts);
            foreach (var p in posts)
            {
                if (LikedPosts.Contains(p)) p.Liked = true;
                else p.Liked = false;
            }
            var categories = await _categorieRepo.GetAll();
            var model = new SearchModel {
                PageTitle = q,
                Categories = categories.Where(x => x.Name.ToLower().Contains(q.ToLower())),
                Posts = posts,
                User = acc,
                Users = await _userRepo.FilterByusername(q, null),
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditImages()
        {
            var acc = await _userManager.GetUserAsync(User);
            if(acc == null ) return RedirectToAction("Index", "Home");
            if (acc.LockoutEnabled) { await _signInManager.SignOutAsync(); return RedirectToPage("/Identity/Account/Lockout"); };
            var files = HttpContext.Request.Form.Files;
            if (files == null || files.Count == 0) RedirectToAction(nameof(Profile), new { id = acc.Id });
            foreach(var f in files)
            {
                if(f.Name == "profileInput")
                {
                    var newFileName = $"{acc.Id}_{Guid.NewGuid()}{Path.GetExtension(f.FileName)}";
                    var uploadFolder = Path.Combine(_host.WebRootPath, "uploads/img/usersImg");

                    if (acc.ProfileImage != "defaultUserImage.jpg")
                    {
                        // Remove existing image
                        var existingFileName = Path.GetFileName(acc.CoverImage);
                        var existingFilePath = Path.Combine(uploadFolder, existingFileName);
                        FileInfo fileInfo = new FileInfo(existingFilePath);
                        if (fileInfo.Exists)
                        {
                            fileInfo.Delete();
                        }
                    }

                    var addr = Path.Combine(uploadFolder, newFileName);
                    using (var fileStream = new FileStream(addr, FileMode.Create))
                    {
                        await f.CopyToAsync(fileStream);
                    }
                    acc.ProfileImage = newFileName;
                    await _userManager.UpdateAsync(acc);
                }
                if (f.Name == "coverInput")
                {
                    var newFileName = $"{acc.Id}_{Guid.NewGuid()}{Path.GetExtension(f.FileName)}";
                    var uploadFolder = Path.Combine(_host.WebRootPath, "uploads/img/profileCover");
                    if(!acc.CoverImage.ToLower().Contains("defaultusercover-"))
                    {
                        // Remove existing image
                        var existingFileName = Path.GetFileName(acc.CoverImage);
                        var existingFilePath = Path.Combine(uploadFolder, existingFileName);
                        FileInfo fileInfo = new FileInfo(existingFilePath);
                        if (fileInfo.Exists)
                        {
                            fileInfo.Delete();
                        }
                    }                   
                    var addr = Path.Combine(uploadFolder, newFileName);
                    using (var fileStream = new FileStream(addr, FileMode.Create))
                    {
                        await f.CopyToAsync(fileStream);
                    }
                    acc.CoverImage = newFileName;
                    await _userManager.UpdateAsync(acc);
                }
                else break;
            }
            return RedirectToAction(nameof(Profile), new {id = acc.Id });
        }
        #endregion

        #region Actions Via Ajax 
        [HttpGet]
        public async Task<JsonResult> DeletePostApi(string ownerId,int postId){
            if (string.IsNullOrWhiteSpace(ownerId) || postId == null || postId == 0) return Json(data: null);            
            var owner = await _userManager.GetUserAsync(User);
            if (owner == null || owner.RoleId != 2) return Json(data: null);
            if (owner.LockoutEnabled) { await _signInManager.SignOutAsync(); return Json(data: null); };
            owner = await _userRepo.Get(ownerId);
            var post = await _postRepo.Get(postId);
            if (post == null) return Json(data: null);
            if(!owner.Posts.Contains(post)) return Json(data: null);
            try
            {
                await _postRepo.Delete(post);
                var imageFolder = Path.Combine(_host.WebRootPath, "uploads/img/postImg");
                var imageUrl = Path.Combine(imageFolder, post.ImageUrl);
                FileInfo file = new FileInfo(imageUrl);
                if (file.Exists) file.Delete();
                return Json(data: new { success = true });
            }catch(Exception ex){
                return Json(data: new { success = false });
            }
        }
        
        [HttpGet]
        public async Task<JsonResult> LoadPosts(string time)
        {
            var acc = await _userManager.GetUserAsync(User);
            if (acc == null) return Json(data: null);
            if (acc.LockoutEnabled) { await _signInManager.SignOutAsync(); return Json(data: null); };
            if (string.IsNullOrWhiteSpace(time)) return Json(data: null);
            acc = await _userRepo.Get(acc.Id);
            var posts =await _postRepo.LoadPosts(time);
            if (posts == null || posts.Count() == 0) return Json(data: null);
            var postList = new List<PostModel>();
            var likedposts = await _postRepo.GetLikedPosts(acc.LikedPosts);
            foreach (var p in posts)
            {
                var i = new PostModel
                {
                    postId=p.id,
                    userId =p.UserId,
                    viewerId=acc.Id,
                    fullName= p.User.FullName,
                    imageUrl=p.ImageUrl,
                    profileImage=p.User.ProfileImage,
                    title = p.Title,
                    likes = p.Likes.Count(),
                    createdTime = p.CreatedTime.ToString("HH:mm dd/MM/yyyy"),
                    coments = p.Coments.Count(),
                    liked = false,
                    owner = false,
                };
                if (likedposts.Contains(p)) i.liked = true;
                if (acc.Posts.Contains(p)) i.owner = true;
                postList.Add(i);
            }
            var lastPost = posts.Last();
            var result = new { posts = postList , time = lastPost.CreatedTime.ToString() }; 
            return Json(data: result);
        }

        [HttpGet]
        public async Task<JsonResult> SearchByUsername(string username)
        {
            var acc = await _userManager.GetUserAsync(User);
            if (acc == null) return Json(data: null);
            if (acc.LockoutEnabled) { await _signInManager.SignOutAsync(); return Json(data: null); };
            if (string.IsNullOrWhiteSpace(username)) return Json(data: null);
            var users = await _userRepo.FilterByusername(username, null);
            if (users == null) return Json(data: null);
            var usersList = new List<SearchUserModel> { };
            foreach (var u in users.Where(u => u.RoleId != 1))
            {
                var i = new SearchUserModel
                {
                    Id = u.Id,
                    Image = u.ProfileImage,
                    UserName = u.UserName,
                };
                usersList.Add(i);
            }
            return Json(data: usersList);
        }

        [HttpGet]
        public async Task<JsonResult> AddToDrafts(string ownerId, int postId)
        {
            if (string.IsNullOrWhiteSpace(ownerId) || postId == null || postId == 0) return Json(data: null);
            var owner = await _userManager.GetUserAsync(User);
            if (owner == null || owner.RoleId != 2) return Json(data: null);
            if (owner.LockoutEnabled) { await _signInManager.SignOutAsync(); return Json(data: null); };
            owner = await _userRepo.Get(ownerId);
            var post = await _postRepo.Get(postId);
            if (post == null) return Json(data: null);
            if (post.UserId != owner.Id) return Json(data: null);
            try
            {
                if(post.PostStatusId == 1) post.PostStatusId = 2;
                else post.PostStatusId = 1;
                List<int> pc = new();
                await _postRepo.Update(post, pc);
                return Json(data: new { success = true });
            }
            catch (Exception ex)
            {
                return Json(data: new { success = false });
            }
        }

        [HttpGet]
        public async Task<JsonResult> AddComent(int postId,string userId, string comentContext)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Json(data: null);
            if (user.LockoutEnabled) { await _signInManager.SignOutAsync(); return Json(data: null); };
            if (user.Id != userId ||string.IsNullOrWhiteSpace(userId) || postId == 0 || postId == null) return Json(data: null);
            var post = await _postRepo.Get(postId);
            if (post == null) return Json(data: null);         
            var comentStatus = await _postRepo.AddComent(postId, userId, comentContext);
            if (!comentStatus) return Json(data: null);
            var coments = await _postRepo.GetComents(postId);
            var comentsList = coments.Select(c => new {
                UserId = c.UserId,
                ComentContext = c.ComentContext,
                CreatedTime = c.CreatedTime.ToString("yyyy/MM/dd, HH:mm"),
                ProfileImage = c.User.ProfileImage,
                UserName = c.User.UserName
            }).ToList();
            return Json(data: comentsList );
        }
        #endregion
    }
}
