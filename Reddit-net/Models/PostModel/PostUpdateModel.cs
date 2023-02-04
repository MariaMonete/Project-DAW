using System.ComponentModel.DataAnnotations;

namespace RedditNet.Models.PostModel
{
    public class PostUpdateModel : PostModel
    {
        [Required(ErrorMessage = "You must add the content")]
        public String Text { get; set; }
        [Required]
        public int Votes { get; set; }
    }
}
