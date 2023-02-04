using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RedditNet.DataLayerFolder;
using RedditNet.Models.DatabaseModel;
using RedditNet.Models.UserModel;
using RedditNet.UserFolder;

namespace RedditNet.Controllers
{
    [Authorize(Roles = "Regular,Moderator,Admin")]
    public class UsersController : Controller
    {

        private DataLayerComments dbComments;
        private DataLayerUsers dbUsers;
        private readonly UserManager<DatabaseUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _context;
        private IWebHostEnvironment _env;
        public UsersController(
            AppDbContext context,
            UserManager<DatabaseUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IWebHostEnvironment
            env)
        {
            _context = context;
            dbComments = new DataLayerComments(context);
            dbUsers = new DataLayerUsers(userManager, roleManager);
            _userManager = userManager;
            _roleManager = roleManager;
            _env = env;
        }

        public async Task<IActionResult> UploadProfilePic(IFormFile profilePic)
        {
            if (profilePic != null && profilePic.Length > 0)
            {
                Console.WriteLine(profilePic.FileName);
                Console.WriteLine(profilePic.Length);
                Console.WriteLine(profilePic.Headers.ContentLength);
                var storagePath = Path.Combine(
                        _env.WebRootPath,
                        "pictures",
                        profilePic.FileName
                        );

                var databaseFileName = "/pictures/" + profilePic.FileName;

                using (var fileStream = new FileStream(storagePath,
                                            FileMode.Create))
                {
                    await profilePic.CopyToAsync(fileStream);
                }
                
                var user = _userManager.FindByEmailAsync(User.Identity.Name).Result;

                user.ProfilePic = databaseFileName;
                _context.SaveChanges();
            }

            return Redirect("/Identity/Account/Manage");
    }

        [Authorize(Roles = "Admin")]
        [HttpPost("users/{userId}/changeRole/{newRole}", Name = "ChangeRoleUser")]
        public async Task<IActionResult> ChangeRole(String userId, String newRole)
        {
            if (User.IsInRole("Admin"))
            {
                DatabaseUser? user = dbUsers.readUser(userId);
                String[] roles = new string[] { "Admin", "Regular", "Moderator" };
                if (user != null)
                {

                    List<String> currentRoles = (List<string>)_userManager.GetRolesAsync(user).Result;
                    /*_userManager.RemoveFromRolesAsync(user, currentRoles);*/
                    foreach (var role in currentRoles)
                    {
                        await _userManager.RemoveFromRoleAsync(user, role);
                    }
                    /*if (!roles.Contains<String>(newRole))
                        newRole = "Regular";*/

                    await _userManager.AddToRoleAsync(user, newRole);
                    await _userManager.UpdateSecurityStampAsync(user);

                    /*var roleName = await _roleManager.FindByIdAsync(newRole);
                    await _userManager.AddToRoleAsync(user, newRole);
*/

                }

                return RedirectToRoute("ShowUser", new { userId = userId });
            }

            return View("Error");
        }

        [HttpGet("users/{userId}", Name = "ShowUser")]
        public IActionResult Show(String userId)
        {
            DatabaseUser? u = dbUsers.readUser(userId);
            if (u != null)
            {
                UserMapper mapper = new UserMapper();
                UserShowModel result = mapper.toShowModel(
                    u,
                    _userManager.GetRolesAsync(u).Result.ElementAt(0)
                    );
                SetAccessRights();

                return View("Show", result);
            }

            return View("Show", null);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("users/{userId}/delete", Name = "DeleteUser")]
        public IActionResult Delete(String userId)
        {
            DatabaseUser? u = dbUsers.readUser(userId);
            Console.WriteLine(u.UserName);
            if (u != null)
            {
                try
                {
                    TempData["message"] = "The user has been deleted";
                    Console.WriteLine(u.UserName);
                    _context.Remove(u);
                    _context.SaveChanges();

                    return Redirect("/");
                }catch(Exception e)
                {

                }
            }

            return Redirect("/");
        }

        //[Authorize(Roles = "Admin")]
        /*    [HttpPost("users/{userId}/changeRole/{newRole}", Name = "ChangeRoleUser")]
            public IActionResult ChangeRole(String userId, String newRole)
            {
                //if (User.IsInRole("Admin"))
                //{
                DatabaseUser? user = dbUsers.readUser(userId);
                String[] roles = new string[] { "Admin", "Regular", "Moderator" };
                if (user != null)
                {
                    List<String> currentRoles = (List<string>)_userManager.GetRolesAsync(user).Result;
                    _userManager.RemoveFromRolesAsync(user, currentRoles);

                    if (!roles.Contains<String>(newRole))
                        newRole = "Regular";

                    _userManager.AddToRoleAsync(user, newRole);
                    _userManager.UpdateSecurityStampAsync(user);



                }

                return RedirectToRoute("ShowUser", new { userId = userId });
                //}

                return View("Error");
            }*/

        [HttpPost("users/edit")]
    public IActionResult Edit(UserUpdateModel m)
    {
        DatabaseUser? u = _userManager.FindByEmailAsync(User.Identity.Name).Result;

        if (u == null)
        {
            return Redirect("/Identity/Account/Manage");
        }

        u.Description = m.Description;
            _context.SaveChanges();

        return Redirect("/Identity/Account/Manage");
    }

        //[HttpPost("users")]
        //public IActionResult Create([FromBody] UserCreateModel u)
        //{
        //    UserMapper mapper = new UserMapper();
        //    User user = mapper.createModelToUser(u);

        //    du.createUser(user);
        //    return Ok();
        //}

        //[HttpDelete("users/{userId}")]
        //public IActionResult Delete(String userId, [FromBody] UserDeleteModel u)
        //{
        //    du.deleteUser(userId, u);
        //    return Ok();
        //}

        //
        private void SetAccessRights()
        {

            ViewBag.AfisareButoane = false;
            if (User.IsInRole("Moderator") || User.IsInRole("Regular"))
            {
                ViewBag.AfisareButoane = true;
            }
            ViewBag.UserCurent = _userManager.GetUserId(User);
            ViewBag.EsteAdmin = User.IsInRole("Admin");
            ViewBag.EsteModerator = User.IsInRole("Moderator");

        }
        //
    }
}
