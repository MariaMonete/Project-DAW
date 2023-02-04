using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RedditNet.CommentFolder;
using RedditNet.DataLayerFolder;
using RedditNet.Models;
using RedditNet.Models.CommentModel;
using RedditNet.Models.DatabaseModel;
using RedditNet.Models.PostModel;
using RedditNet.Models.SubRedditModel;
using RedditNet.Models.UserModel;
using RedditNet.PostFolder;
using RedditNet.UtilityFolder;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography.Xml;

namespace RedditNet.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private DataLayerPosts dbPosts;
        private DataLayerSubReddits dbSubs;
        private DataLayerComments dbComments;
        private DataLayerUsers dbUsers;

        public HomeController(AppDbContext context,
            ILogger<HomeController> logger,
            UserManager<DatabaseUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            dbPosts = new DataLayerPosts(context);
            dbSubs = new DataLayerSubReddits(context);
            dbComments = new DataLayerComments(context);
            dbUsers = new DataLayerUsers(userManager, roleManager);

            _logger = logger;
        }

        public IActionResult Index()
        {
            (List<SubRedditPreviewModel>? result, int cnt) = dbSubs.getSubs();
            if (result != null)
            {
                if (TempData.ContainsKey("message"))
                {
                    ViewBag.message = TempData["message"].ToString();
                }
                return View(result);
            }
            return View("Error");
        }

        [HttpGet("search/{page}/{type}", Name = "SearchResults")]
        public IActionResult Search(int page, String type)
        {
            SearchModel sm = new SearchModel();
            String search = Convert.ToString(HttpContext.Request.Query["search"]).Trim();

            int cnt = 0;
            if (type == Constants.subType)
            {
                (List<SubRedditPreviewModel>? subs, cnt) = dbSubs.getSubs(search, page);
                sm.Subs = subs;
            }else if (type == Constants.comType)
            {
                (List<CommentThreadModel>? coms, cnt) = dbComments.getComments(search, page);
                sm.Comments = coms;
            }else if (type == Constants.postType)
            {
                (List<PostPreviewModel>? posts, cnt) = dbPosts.getPosts(search, page);
                sm.Posts = posts;
            }else if (type == Constants.userType)
            {
                (List<UserShowModel>? users, cnt) = dbUsers.getUsers(search, page);
                sm.Users = users;
            }

            ViewBag.Page = page;
            ViewBag.MaxPage = Math.Ceiling((float)cnt / (float)Constants.pageSizePosts) - 1;

            return View(sm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}