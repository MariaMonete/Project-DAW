using RedditNet.Models.CommentModel;

namespace RedditNet.Models.PostModel
{
    public class PostThreadModel : PostModel
    {
        public String Text { get; set; }

        public int? Votes { get; set; }
        public String SubName { get; set; }
        public String UserName { get; set; }

        public List<CommentThreadModel> Comments { get; set; }
    }
}
