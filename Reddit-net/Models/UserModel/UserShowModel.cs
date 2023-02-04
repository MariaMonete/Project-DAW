namespace RedditNet.Models.UserModel
{
    public class UserShowModel : UserModel
    {
        public String UserName { get; set; }
        public String? Description { get; set; }
        public String Email { get; set; }
        public String Role { get; set; }
        public String? ProfilePic { get; set; }
    }
}
