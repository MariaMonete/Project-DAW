using System.ComponentModel.DataAnnotations;

namespace RedditNet.Models.CommentModel
{
    public class CommentCreateModel : CommentModel
    {
        public int Parent { get; set; }
        [Required(ErrorMessage = "You can't leave an empty comment")]
        public String Text { get; set; }
    }
}
