using Blog.Areas.Identity.Data;
using Blog.Models;
using Blog.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace Blog.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IUserRepo _userRepo;
        private readonly IPostRepo _postRepo;
        private readonly ILikeRepo _likeRepo;
        private readonly IHostingEnvironment _host;

        public HomeController(
            ILogger<HomeController> logger,
            UserManager<User> userManager,
            IUserRepo userRepo,
            IPostRepo postRepo,
            SignInManager<User> signInManager,
            ILikeRepo likeRepo,
            IHostingEnvironment host)
        {
            _logger = logger;
            _userManager = userManager;
            _userRepo = userRepo;
            _postRepo = postRepo;
            _signInManager = signInManager;
            _likeRepo = likeRepo;
            _host = host;
        }

        public async Task<IActionResult> Index()
        {
            var acc = await _userManager.GetUserAsync(User);
            if (acc == null)
            {
                await _signInManager.SignOutAsync();
                return Redirect("~/");
            }
            if (acc.LockoutEnabled) { await _signInManager.SignOutAsync(); return RedirectToPage("/Identity/Account/Lockout"); };
            if (acc.RoleId == 1)
            {
                return RedirectToAction("Index", "Admin");
            }
            else if (acc.RoleId == 2 || acc.RoleId == 3)
            {
                return RedirectToAction("Index", "Blog");
            }
            return View();

        }

        


        [HttpGet]
        public async Task<JsonResult> FilterUserPosts(string userId, int? categorieId, string? fromTime, string? toTime)
        {
            if (categorieId == 0 && string.IsNullOrWhiteSpace(fromTime) && string.IsNullOrWhiteSpace(toTime)) return Json(data: null);
            var user = await _userRepo.Get(userId);
            if(user == null) return Json(data: null);
            if (user.LockoutEnabled) { await _signInManager.SignOutAsync(); return Json(data: null); };

            var posts = await _postRepo.FilterUserPosts(userId,categorieId,fromTime,toTime);
            if (posts == null) return Json(data: null);

            var viewer = await _userRepo.Get(_userManager.GetUserId(User));
            var likedPosts = await _postRepo.GetLikedPosts(viewer.LikedPosts);

            var postList = new List<FilterUserPosts> { };
            foreach (var p in posts.OrderByDescending(d => d.CreatedTime))
            {
                var post = new FilterUserPosts
                {
                    postId = p.id,
                    userId = p.UserId,
                    viewerId = _userManager.GetUserId(User),
                    imageUrl = p.ImageUrl,
                    profileImage = p.User.ProfileImage,
                    title = p.Title,
                    fullName = p.User.FullName,
                    createdTime = p.CreatedTime.ToString("HH:mm dd/MM/yyyy"),
                    likes = p.Likes.Count,
                    coments = p.Coments.Count(),
                    liked = false,
                };
                if (likedPosts.Contains(p)) post.liked = true;
                postList.Add(post);
            }
            return Json(data: postList);
        }

        [HttpGet]
        public async Task<JsonResult> GetUserPosts(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) return Json(data: null);
            var user = await _userRepo.Get(userId);
            if (user == null) return Json(data: null);
            if (user.LockoutEnabled) { await _signInManager.SignOutAsync(); return Json(data: null); };


            var posts = await _postRepo.GetUserPosts(userId);
            if (posts == null) return Json(data: null);

            var viewer = await _userRepo.Get(_userManager.GetUserId(User));
            var likedPosts = await _postRepo.GetLikedPosts(viewer.LikedPosts);

            var postList = new List<FilterUserPosts> { };
            foreach (var p in posts.OrderByDescending(d => d.CreatedTime))
            {
                var post = new FilterUserPosts
                {
                    postId = p.id,
                    userId = p.UserId,
                    viewerId = _userManager.GetUserId(User),
                    imageUrl = p.ImageUrl,
                    profileImage = p.User.ProfileImage,
                    title = p.Title,
                    fullName = p.User.FullName,
                    createdTime = p.CreatedTime.ToString("HH:mm dd/MM/yyyy"),
                    likes = p.Likes.Count,
                    coments = p.Coments.Count(),
                    liked = false,
                };
                if (likedPosts.Contains(p)) post.liked = true;
                postList.Add(post);
            }
            return Json(data: postList);
        }

        [HttpGet]
        public async Task<JsonResult> Like(string userId, int postId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || user.Id != userId) return Json(data: null);
            if (user.LockoutEnabled) { await _signInManager.SignOutAsync(); return Json(data: null); };
            if (string.IsNullOrWhiteSpace(userId) || postId == 0 || postId == null || postId < 1)return Json(data: null);
            var result = await _likeRepo.Like(userId, postId);
            if (!result) return Json(data: null);
            var likeCount = await _postRepo.GetLikeCount(postId);
            if (likeCount < 0) return Json(data: null);
            var response = new { likes = likeCount };
            return Json(data: response);
        }

    }
}