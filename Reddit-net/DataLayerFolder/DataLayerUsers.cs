using Microsoft.AspNetCore.Identity;
using RedditNet.Models.DatabaseModel;
using RedditNet.Models.UserModel;
using RedditNet.UserFolder;
using RedditNet.UtilityFolder;

namespace RedditNet.DataLayerFolder
{

    public class DataLayerUsers
    {
        private readonly UserManager<DatabaseUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public DataLayerUsers(
            UserManager<DatabaseUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        //private bool hasPermission(User affectedUser, User requestingUser)
        //{
        //    return affectedUser.isSame(requestingUser) || requestingUser.isAdmin();
        //}
        //public void createUser(User u)
        //{
        //    if (!DatabaseInterface.users.ContainsKey(u.Id))
        //    {
        //        DatabaseInterface.users[u.Id] = u;
        //    }
        //}

        public DatabaseUser? readUser(String id)
        {
            try
            {
                DatabaseUser? p = (from b in _userManager.Users
                                        where (b.Id == id)
                                        select b).FirstOrDefault<DatabaseUser>();

                return p;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public (List<UserShowModel>?, int) getUsers(String search, int start = 0)
        {
            if (search != "")
            {
                try
                {
                    var dbu = (from b in _userManager.Users
                               where (b.UserName.Contains(search))
                               select b).OrderBy(o => o.Id);
                    List<DatabaseUser>? users = dbu.Skip(start * Constants.pageSizePosts).Take(Constants.pageSizePosts).ToList();

                    int cnt = dbu.Count();

                    List<UserShowModel>? result = new List<UserShowModel>();
                    foreach (var u in users)
                    {
                        UserShowModel usm = new UserShowModel();
                        usm.UserName = u.UserName;
                        usm.Id = u.Id;

                        result.Add(usm);
                    }

                    return (result, cnt);
                }
                catch (Exception)
                {

                }
            }

            return (null, 0);
        }

        //public void updateUser(String id, UserUpdateModel u)
        //{
        //    User updatedUser = readUser(id);
        //    User otherUser = readUser(u.Id);

        //    if (updatedUser != null && otherUser != null)
        //    {
        //        if (hasPermission(updatedUser, otherUser))
        //            updatedUser.update(u);
        //        if (otherUser.isAdmin())
        //            updatedUser.updateRole(u.Role);

        //    }
        //}

        //public void deleteUser(String id, UserDeleteModel u)
        //{
        //    User deletedUser = readUser(id);
        //    User otherUser = readUser(u.Id);
        //    if (deletedUser != null && otherUser != null)
        //    {
        //        if (deletedUser.isSame(u) || otherUser.isAdmin())
        //        {
        //            DatabaseInterface.users.Remove(id);
        //        }
        //    }
        //}
    }
}
