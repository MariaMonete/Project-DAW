using Ganss.Xss;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using RedditNet.CommentFolder;
using RedditNet.DataLayerFolder;

using RedditNet.Models.CommentModel;
using RedditNet.Models.DatabaseModel;
using RedditNet.Models.PostModel;
using RedditNet.Models.SubRedditModel;
using RedditNet.PostFolder;
using RedditNet.SubRedditFolder;
using RedditNet.UserFolder;
using RedditNet.UtilityFolder;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.Arm;

namespace RedditNet.Controllers
{
    public class PostsController : Controller
    {
        private DataLayerPosts dbPosts;
        private DataLayerComments dbComments;
        private DataLayerSubReddits dbSubs;
        private DataLayerUsers dbUsers;

        private readonly UserManager<DatabaseUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public PostsController(
            AppDbContext context,
            UserManager<DatabaseUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            dbPosts = new DataLayerPosts(context);
            dbComments = new DataLayerComments(context);
            dbSubs = new DataLayerSubReddits(context);
            dbUsers = new DataLayerUsers(userManager, roleManager);

            _userManager = userManager;
            _roleManager = roleManager;
        }

        //TODO
        //Use [deleted] for posts with deleted user
        //Add edit for admins
        //Introduce tokens for authentication

        
        public IActionResult Index()
        {
            return View();
        }

        
        
        [HttpGet("{subId}/posts/{postId}/comments/{sortType?}", Name = "ShowPostComments")]
        public IActionResult Show(String subId, String postId, int? sortType)
        {

            List<CommentNode>? nodes = dbComments.getDescendants(postId, -1, sortType == null ? Constants.comparisonByTimeDesc : (int)sortType);
            if (nodes == null)
                return View("Error");

            List<CommentThreadModel> comments = new List<CommentThreadModel>();
            foreach (CommentNode n in nodes)
            {
                DatabaseMapper mapper = new DatabaseMapper();
                DatabaseComment? dbc = dbComments.readComment(postId, n.Id);
                
                if (dbc != null)
                {
                    comments.Add(mapper.toThreadComment(dbc, subId));
                }
            }

            DatabasePost? dbPost = dbPosts.readPost(subId, postId);
            
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }
            
            if (dbPost != null)
            {
                PostMapper pmapper = new PostMapper();
                DatabaseMapper databaseMapper = new DatabaseMapper();

                DatabaseSubReddit? sub = dbSubs.readSubReddit(subId);
                PostThreadModel result = pmapper.toThreadModel(
                    comments, 
                    databaseMapper.toPost(dbPost), sub == null ? "" : sub.Name,
                    dbPost.User?.UserName ?? "[deleted]");

                ViewBag.ByTimeAsc = Constants.comparisonByTimeAsc;
                ViewBag.ByTimeDesc = Constants.comparisonByTimeDesc;
                ViewBag.ByVotesAsc = Constants.comparisonByVotesAsc;
                ViewBag.ByVotesDesc = Constants.comparisonByVotesDesc;

                SetAccessRights();
                
                return View("Show", result);
            }

            return View("Error");
        }

        //
        private void SetAccessRights()
        {
            
            ViewBag.AfisareButoane = false;
            if (User.IsInRole("Moderator")||User.IsInRole("Regular")||User.IsInRole("Admin"))
            {
                ViewBag.AfisareButoane = true;
            }
            ViewBag.UserCurent = _userManager.GetUserId(User);
            ViewBag.EsteAdmin = User.IsInRole("Admin");
            ViewBag.EsteModerator = User.IsInRole("Moderator");
            
        }
        //

        [Authorize(Roles = "Regular,Moderator,Admin")]
        [HttpPost("{subId}/posts/edit/{postId}")]
        public IActionResult Edit(String subId, String postId, PostUpdateModel p)
        {
            var sanitizer=new HtmlSanitizer();
            DatabasePost? dbPost = dbPosts.updatePost(subId, postId, p);

            if (dbPost != null && p.Text != null)
            {
                if (_userManager.GetUserId(User)==dbPost.User.Id || User.IsInRole("Admin"))
                {
                    dbPost.Text = sanitizer.Sanitize(p.Text);
                    TempData["message"] = "Your post has been modified";

                    return RedirectToAction("Show", new { subId = dbPost.SubId, postId = dbPost.Id });
                }
                else
                {
                    TempData["message"] = "You can't edit a post that is not yours";
                    return RedirectToAction("Show", new { subId = dbPost.SubId, postId = dbPost.Id });
                }
                
            }

            else
            {
                PostUpdateModel pm = new PostUpdateModel();
                
                pm.SubId =subId;
                pm.UserId = _userManager.GetUserId(User);
                pm.Id = postId;
                pm.Title = p.Title;
               
                return View(pm);
            }    


            /*var dbPost = await dbPosts.UpdatePost(subId, postId,p);

            if(p.Text==null)
            {
                //mesaj validare care va fi afisat in show
                TempData["message"] = "Your post cannot be changed";
                //
                return RedirectToAction("Show", new { subId = dbPost.SubId, postId = dbPost.Id });
                
            }

            //mesaj validare care va fi afisat in show
            TempData["message"] = "Your post has been modified";
            //
            return RedirectToAction("Show", new { subId = dbPost.SubId, postId = dbPost.Id });*/

            /*DatabasePost? dbPost = dbPosts.updatePost(subId, postId, p);
            
            if (dbPost == null|| p.Text == null)
            {
                return View(p);
            }
            
            //mesaj validare care va fi afisat in show
            TempData["message"] = "Your post has been modified";
            //
            return RedirectToAction("Show", new { subId = dbPost.SubId, postId = dbPost.Id });*/



        }
        [Authorize(Roles ="Regular,Moderator,Admin")]
        [HttpGet("{subId}/posts/edit/{postId}", Name = "EditPost")]
        public IActionResult EditForm(String subId, String postId)
        {
            DatabasePost? post = dbPosts.readPost(subId, postId);
            DatabaseUser? user = dbUsers.readUser(_userManager.GetUserId(User));


            if (post != null)
            {
                PostUpdateModel pm = new PostUpdateModel();
                pm.Text = post.Text;
                pm.SubId = post.SubId;
                pm.UserId = post.User.Id;
                pm.Votes = post.Votes;
                pm.Id = post.Id;
                pm.Title = post.Title;
                
                if (pm.UserId==_userManager.GetUserId(User)|| User.IsInRole("Admin"))
                {
                    
                    return View("Edit", pm);
                }
                else
                {
                    TempData["message"] = "You can't edit a post that is not yours";
                    return RedirectToAction("Show", new { subId = pm.SubId, postId = pm.Id });
                }

                
            }
            return View("Error");
        }

        [Authorize(Roles = "Regular,Moderator,Admin")]
        [HttpPost("{subId}/posts/create")]
        public IActionResult Create(PostCreateModel p)
        {
            var sanitizer = new HtmlSanitizer();
            PostMapper mapper = new PostMapper();
            Post post = mapper.createModelToPost(p);

            DatabaseUser? user = dbUsers.readUser(_userManager.GetUserId(User));
            Console.WriteLine(user.Id);
            DatabasePost? dbPost = dbPosts.createPost(post, user);
            if (dbPost != null)
            {
                dbPost.Text = sanitizer.Sanitize(dbPost.Text);
                //mesaj validare care va fi afisat in show
                TempData["message"] = "Your post has been added";
                //
                return RedirectToAction("Show", new { subId = dbPost.SubId, postId = dbPost.Id });
            }
            else
            {
                return View(p);
            }

            
        }

        [Authorize(Roles = "Regular,Moderator,Admin")]
        [HttpGet("{subId}/posts/create")]
        public IActionResult CreateForm(String subId)
        {
            var sanitizer = new HtmlSanitizer();
            PostCreateModel cm = new PostCreateModel();
            cm.Title = "";
            cm.Text = "";
            cm.UserId = _userManager.GetUserId(User);
            cm.SubId = subId;

            

            return View("Create", cm);
        }

        [Authorize(Roles ="Regular,Moderator,Admin")]
        [HttpPost("{subId}/posts/delete/{postId}")]
        public IActionResult Delete(String subId, String postId, PostDeleteModel p)
        {
            DatabasePost? post = dbPosts.readPost(subId, postId);
            if (post.User.Id == _userManager.GetUserId(User) || User.IsInRole("Admin")|| User.IsInRole("Moderator"))
            {
                if (dbPosts.deletePost(subId, postId, p) == true)
                {
                    TempData["message"] = "The post has been deleted";

                    return Redirect("/subs/" + subId + "/posts/0");
                }
                return BadRequest();
            }
            else
            {
                TempData["message"] = "You can't delete a post that is not yours";

                return Redirect("/subs/" + subId + "/posts/0");
            }
                
        }

        [Authorize(Roles = "Regular,Moderator,Admin")]
        [HttpGet("{subId}/posts/delete/{postId}", Name = "DeletePost")]
        public IActionResult DeleteForm(String subId, String postId, PostDeleteModel p)
        {
            DatabasePost? post = dbPosts.readPost(subId, postId);
            if (post != null)
            {
                PostDeleteModel pm = new PostDeleteModel();
                pm.SubId = post.SubId;
                pm.UserId = post.User.Id;
                pm.Id = post.Id;
                pm.Title = post.Title;

                if (pm.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin")|| User.IsInRole("Moderator"))
                    return View("Delete", pm);
                else
                {
                    TempData["message"] = "You can't delete a post that is not yours";
                    return Redirect("/subs/" + subId + "/posts/0");
                }
                
            }

            return View("Error");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{subId}/{oldSubId}/posts/move/{postId}")]
        public IActionResult Move(String subId, String oldSubId, String postId)
        {
            bool res = dbPosts.movePost(subId, oldSubId, postId);    
            if (res == true)
            {
                TempData["message"] = "The post has been moved";
                return RedirectToAction("Show", new { subId = subId, postId = postId });
            }

            return BadRequest();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{subId}/posts/move/{postId}", Name = "MovePost")]
        public IActionResult MoveForm(String subId, String postId)
        {
            (List<SubRedditPreviewModel>? result, int cnt) = dbSubs.getSubs();
            if (result != null)
            {
                if (TempData.ContainsKey("message"))
                {
                    ViewBag.message = TempData["message"].ToString();
                }

                ViewBag.SubId = subId;
                ViewBag.PostId = postId;
                result.RemoveAll(r => r.Id == subId);
                return View("Move", result);
            }
            return View("Error");
        }
    }
}
