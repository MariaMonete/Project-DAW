namespace RedditNet.Models.UserModel
{
    public class UserUpdateModel : UserModel
    {
        public String? Password { get; set; }

        public String? Description { get; set; }

        public String? Email { get; set; }
        public int? Role { get; set; }
    }
}
