using Microsoft.AspNetCore.Identity;

namespace RedditNet.Models.DatabaseModel
{
    public class DatabaseUser : IdentityUser
    {
        public String? ProfilePic { get; set; }
        public String? Description { get; set; }
    }
}
