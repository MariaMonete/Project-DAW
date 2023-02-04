using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using RedditNet.DataLayerFolder;
using RedditNet.Models.DatabaseModel;
using RedditNet.Models.PostModel;
using RedditNet.Models.SubRedditModel;
using RedditNet.PostFolder;
using RedditNet.SubRedditFolder;
using RedditNet.UtilityFolder;

namespace RedditNet.Controllers
{
    
    public class SubRedditsController : Controller
    {
        private DataLayerPosts dbPosts;
        private DataLayerSubReddits dbSubs;
        private DataLayerUsers dbUsers;
        private readonly UserManager<DatabaseUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public SubRedditsController(
            AppDbContext context,
            UserManager<DatabaseUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            dbPosts = new DataLayerPosts(context);
            dbSubs = new DataLayerSubReddits(context);
            dbUsers = new DataLayerUsers(userManager, roleManager);
            dbUsers = new DataLayerUsers(userManager, roleManager);

            _userManager = userManager;
            _roleManager = roleManager;
        }
        
        
    

    [HttpGet("subs/{subId}/posts/{start}", Name = "ShowSubPosts")]
        public IActionResult Show(String subId, int start)
        {
            DatabaseSubReddit? s = dbSubs.readSubReddit(subId);
            if (s != null)
            {
                (List<PostPreviewModel>? pm, int cnt) = dbSubs.getPosts(subId, start);
                if (pm != null)
                {

                    ///
                    if (TempData.ContainsKey("message"))
                    {
                        ViewBag.message = TempData["message"].ToString();
                    }
                    ///
                    SubRedditMapper mapper = new SubRedditMapper();
                    DatabaseMapper dmapper = new DatabaseMapper();
                    SubReddit sub = dmapper.toSubReddit(s);

                    SubRedditPostsModel result = mapper.toPostsModel(pm, sub);

                    ViewBag.Page = start;
                    ViewBag.MaxPage = Math.Ceiling((float)cnt /(float)Constants.pageSizePosts) - 1;


                    return View("Show", result);
                }
            }

            return View("Error");
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("subs/edit/{subId}")]
        public IActionResult Edit(String subId, SubRedditUpdateModel m)
        {
            DatabaseSubReddit? dbSub = dbSubs.updateSubReddit(subId, m);
            if (dbSub != null)
            {
                TempData["message"] = "This SubReddit has been edited";
                return RedirectToAction("Show", new { subId = dbSub.Id, start = 0 });
            }
            else
            {
                SubRedditUpdateModel pm = new SubRedditUpdateModel();
                
                pm.Id = subId;
                pm.UserId = _userManager.GetUserId(User);
                return View(pm);
            }
            
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("subs/edit/{subId}", Name = "EditSub")]
        public IActionResult EditForm(String subId)
        {
            DatabaseSubReddit? sub = dbSubs.readSubReddit(subId);
            if (sub != null)
            {
                SubRedditUpdateModel pm = new SubRedditUpdateModel();
                pm.Description = sub.Description;
                pm.Name = sub.Name;
                pm.Id = sub.Id;
                pm.UserId = _userManager.GetUserId(User);

                return View("Edit", pm);
            }
            return View("Error");
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("subs/create")]
        public IActionResult Create(SubRedditCreateModel m)
        {
            SubRedditMapper mapper = new SubRedditMapper();

            Console.WriteLine(m);
            if (m.Name != null&&m.Description!=null)
            {
                SubReddit s = mapper.createModelToSubReddit(m);

                DatabaseSubReddit? res = dbSubs.createSubReddit(s, m.UserId);
                TempData["message"] = "The SubReddit has been added";
                return RedirectToAction("Show", new { subId = res.Id, start = 0 });
            }
            else
            {
                SubRedditCreateModel cm = new SubRedditCreateModel();
                cm.UserId = _userManager.GetUserId(User);
                
                return View(cm);
            }
            
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("subs/create")]
        public IActionResult CreateForm()
        {
            SubRedditCreateModel cm = new SubRedditCreateModel();
            cm.UserId = "";
            cm.Name = "";
            cm.Description = "";

            return View("Create", cm);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("subs/delete/{subId}")]
        public IActionResult Delete(String subId, SubRedditDeleteModel m)
        {
            if (dbSubs.deleteSubReddit(subId, m.UserId) == true)
            {
                TempData["message"] = "The SubReddit has been deleted";
                return Redirect("/Home/Index");
            }
            else
            {
                TempData["message"] = "There was a problem";
                return Redirect("/Home/Index");
            }
            
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("subs/delete/{subId}", Name = "DeleteSub")]
        public IActionResult DeleteForm(String subId)
        {
            DatabaseSubReddit? sub = dbSubs.readSubReddit(subId);
            if (sub != null)
            {
                SubRedditDeleteModel pm = new SubRedditDeleteModel();
                pm.Id = sub.Id;
                pm.Name = sub.Name;
                pm.UserId = "user id here";

                return View("Delete", pm);
            }

            return View("Error");
        }
    }
}
