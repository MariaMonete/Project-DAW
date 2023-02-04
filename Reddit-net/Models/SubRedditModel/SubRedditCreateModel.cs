using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace RedditNet.Models.SubRedditModel
{
    public class SubRedditCreateModel
    {
        public String? UserId { get; set; }
        [Required(ErrorMessage="You must add a title")]
        public String Name { get; set; }
        [Required(ErrorMessage = "You must add a description")]
        public String Description { get; set; }
    }
}
