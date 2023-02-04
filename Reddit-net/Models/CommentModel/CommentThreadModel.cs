namespace RedditNet.Models.CommentModel
{
    public class CommentThreadModel : CommentModel
    {
        public String Text { get; set; }
        public int Depth { get; set; }
        public int? Votes { get; set; }
        public String UserName { get; set; }
        public bool Deleted { get; set; }
    }
}
