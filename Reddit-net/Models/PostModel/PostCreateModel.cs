using Microsoft.Build.Framework;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace RedditNet.Models.PostModel
{
    public class PostCreateModel : PostModel
    {
        [Required(ErrorMessage = "You must add the content")]
        public String Text { get; set; }

    }
}
