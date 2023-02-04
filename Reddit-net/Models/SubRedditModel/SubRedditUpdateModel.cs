

using Microsoft.Build.Framework;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace RedditNet.Models.SubRedditModel
{
    public class SubRedditUpdateModel : SubRedditModel
    {
        public String? UserId { get; set; }
        [Required(ErrorMessage = "You must add a description")]
        public String Description { get; set; }

    }
}
