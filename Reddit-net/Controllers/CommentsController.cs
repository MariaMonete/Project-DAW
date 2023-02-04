using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RedditNet.CommentFolder;
using RedditNet.DataLayerFolder;
using RedditNet.Models.CommentModel;
using RedditNet.Models.DatabaseModel;
using RedditNet.PostFolder;
using RedditNet.UtilityFolder;
using System.Xml.Linq;

namespace RedditNet.Controllers
{
    public class CommentsController : Controller
    {
        private DataLayerComments dbComments;
        private DataLayerUsers dbUsers;
        private readonly UserManager<DatabaseUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public CommentsController(
            AppDbContext context,
            UserManager<DatabaseUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            dbComments = new DataLayerComments(context);
            dbUsers = new DataLayerUsers(userManager, roleManager);
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet("{subId}/{postId}/comments/{commentId}")]
        public IActionResult Show(String subId, String postId, int commentId)
        {
            DatabaseComment? dbComment = dbComments.readComment(postId, commentId);
            if (dbComment != null)
            {
                DatabaseMapper mapper = new DatabaseMapper();
                CommentThreadModel cm = mapper.toThreadComment(dbComment, subId);
                
                return View(cm);
            }
            
            return View("Error");
        }
        
        [Authorize(Roles ="Regular,Moderator,Admin")]
        [HttpPost("{subId}/{postId}/comments/edit/{commentId}")]
        public IActionResult Edit(String subId, String postId, int commentId, CommentUpdateModel c)
        {
            DatabaseComment? result = dbComments.updateComment(postId, commentId, c);

            if (result != null)
            {
                if (_userManager.GetUserId(User) == result.User.Id || User.IsInRole("Admin"))
                {
                    TempData["message"] = "Your comment has been modified";
                    return RedirectToRoute("ShowPostComments", new { subId = subId, postId = postId });
                }
                else
                {
                    TempData["message"] = "You can't edit a comment that is not yours";
                    return RedirectToRoute("ShowPostComments", new { subId = subId, postId = postId });
                }
            }
            else
            {
                CommentUpdateModel cm = new CommentUpdateModel();
                cm.PostId = postId;
                cm.UserId = _userManager.GetUserId(User);
                
                cm.Id =commentId;
                cm.SubId = subId;
                return View(cm);
            }
            
        }
        [Authorize(Roles = "Regular,Moderator,Admin")]
        [HttpGet("{subId}/{postId}/comments/edit/{commentId}", Name = "EditComment")]
        public IActionResult EditForm(String subId, String postId, int commentId)
        {
            DatabaseComment? dbc = dbComments.readComment(postId, commentId);
            if (dbc != null)
            {
                CommentUpdateModel cm = new CommentUpdateModel();
                cm.PostId = dbc.PostId;
                cm.UserId = dbc.User.Id;
                cm.Text = dbc.Text;
                cm.Votes = dbc.Votes;
                cm.Id = dbc.Id;
                cm.SubId = subId;

                if (cm.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
                    return View("Edit", cm);
                else
                {
                    TempData["message"] = "You can't edit a comment that is not yours";
                    return RedirectToRoute("ShowPostComments", new { subId = subId, postId = postId });
                }
            }

            return View("Error");
        }


        [Authorize(Roles = "Regular, Moderator, Admin")]
        [HttpGet("{subId}/{postId}/comments/reply/{commentId}", Name = "CreateComment")]

        public IActionResult CreateForm(String subId, String postId, int commentId)
        {
            CommentCreateModel cm = new CommentCreateModel();
            cm.PostId = postId;
            cm.Parent = commentId;
            cm.SubId = subId;
            cm.UserId = _userManager.GetUserId(User);

            return View("Create", cm);
            
        }

        [HttpGet("{subId}/{postId}/comments")]
        public IActionResult ListComments(String subId, String postId)
        {
            List<CommentNode>? nodes = dbComments.getDescendants(postId);
            if (nodes == null)
                return View("Error");

            List<CommentThreadModel> result = new List<CommentThreadModel>();
            foreach (CommentNode n in nodes)
            {
                DatabaseMapper mapper = new DatabaseMapper();
                DatabaseComment? dbc = dbComments.readComment(postId, n.Id);
                if (dbc != null)
                {
                    result.Add(mapper.toThreadComment(dbc, subId));
                }
            }

            return View("AllComments", result);
        }

        [Authorize(Roles = "Regular, Moderator, Admin")]
        [HttpPost("{subId}/{postId}/comments/{commentId}")]
        public IActionResult Create(String subId, String postId, int commentId, CommentCreateModel c)
        {
            DatabaseUser? user = dbUsers.readUser(_userManager.GetUserId(User));
            if (user == null)
                return BadRequest();

            CommentMapper mapper = new CommentMapper();
            Comment comment = mapper.createToComment(postId, c);
            CommentNode n = new CommentNode(0, commentId);

            DatabaseComment? dbComment = dbComments.createComment(n, comment, user);
            /*Console.WriteLine(dbComment.Text);*/
            
            if (dbComment != null)

            {
                TempData["message"] = "Yor comment has been posted";
                Console.WriteLine(dbComment.Text);
                return RedirectToRoute("ShowPostComments", new { subId = subId, postId = postId });
             
            }
            else
            {
               CommentCreateModel c1=new CommentCreateModel();
                
                c1.PostId = postId;
                c1.Parent = commentId;
                c1.SubId = subId;
                c1.UserId = _userManager.GetUserId(User);
                return View(c1);
            }
                
        }
        [Authorize(Roles = "Regular,Moderator,Admin")]
        [HttpPost("{subId}/{postId}/comments/delete/{commentId}")]
        public IActionResult Delete(String subId, String postId, int commentId, CommentDeleteModel c)
        {
            DatabaseComment? dbc = dbComments.readComment(postId, commentId);
            if (dbc.User.Id == _userManager.GetUserId(User) || User.IsInRole("Admin") || User.IsInRole("Moderator"))
            {
                if (dbComments.deleteComment(postId, commentId, c) == true)
                {
                    TempData["message"] = "The comment has been deleted";
                    return RedirectToRoute("ShowPostComments", new { subId = subId, postId = postId });
                }
                return BadRequest();
            }
            else
            {
                TempData["message"] = "You can't delete a comment ";

                return RedirectToRoute("ShowPostComments", new { subId = subId, postId = postId });
            }
        }
        [Authorize(Roles = "Regular,Moderator,Admin")]
        [HttpGet("{subId}/{postId}/comments/delete/{commentId}", Name = "DeleteComment")]
        public IActionResult DeleteForm(String subId, String postId, int commentId)
        {
            DatabaseComment? dbc = dbComments.readComment(postId, commentId);
            if (dbc != null)
            {
                CommentDeleteModel cm = new CommentDeleteModel();
                cm.PostId = dbc.PostId;
                cm.UserId = dbc.User.Id;
                cm.Id = dbc.Id;
                cm.SubId = subId;

                if (cm.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin") || User.IsInRole("Moderator"))
                {

                    return View("Delete", cm);
                }
                else
                {
                    TempData["message"] = "You can't delete a comment that is not yours";
                    return RedirectToRoute("ShowPostComments", new { subId = subId, postId = postId });
                }
            }

            return View("Error");
        }

    }
}
