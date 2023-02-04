
using System.ComponentModel.DataAnnotations;

namespace RedditNet.Models.PostModel
{
    public class PostModel
    {
        public String UserId { get; set; }

        public String SubId { get; set; }

        public String Id { get; set; }

        [Required(ErrorMessage = "You must add the tile")]
        public String Title { get; set; }
    }
}
