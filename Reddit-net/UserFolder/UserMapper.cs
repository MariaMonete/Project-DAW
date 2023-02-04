
using RedditNet.Models.DatabaseModel;
using RedditNet.Models.UserModel;
using RedditNet.UtilityFolder;

namespace RedditNet.UserFolder
{
    public class UserMapper
    {
        private String roleToString(int? role)
        {
            switch (role)
            {
                case Constants.admin:
                    return "admin";
                case Constants.mod:
                    return "mod";
                case Constants.regular:
                    return "regular";
                default:
                    return "guest";
            }
        }
        public UserShowModel toShowModel(DatabaseUser u, string role)
        {
            UserShowModel result = new UserShowModel();
            result.Id = u.Id;
            result.UserName = u.UserName;
            result.Role = role;
            result.Email = u.Email;
            result.Description = u.Description;
            result.ProfilePic = u.ProfilePic;

            return result;
        }

        //public User createModelToUser(UserCreateModel u)
        //{
        //    User result = new User(u.UserName, u.Password, u.Email, u.Description, Constants.regular);

        //    return result;
        //}
    }
}
