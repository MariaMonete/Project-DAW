using System.ComponentModel.DataAnnotations;

namespace RedditNet.Models.CommentModel
{
    public class CommentUpdateModel : CommentModel
    {
        [Required(ErrorMessage = "The content of the comment is required")]
        public String Text { get; set; }
        [Required]
        public int Votes { get; set; }
    }
}
