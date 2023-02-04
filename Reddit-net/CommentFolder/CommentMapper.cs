using RedditNet.Models.CommentModel;

namespace RedditNet.CommentFolder
{
    public class CommentMapper
    {
        public CommentThreadModel toThreadModel(Comment c, String subId, bool deletedState, int depth = 0, String userName = "user name here")
        {
            CommentThreadModel result = new CommentThreadModel();
            result.Text = c.Text;
            result.UserId = c.UserId;
            result.Id = c.Id;
            result.Depth = depth;
            result.Votes = c.Votes;
            result.SubId = subId;
            result.PostId = c.PostId;
            result.UserName = userName;
            result.Deleted = deletedState;

            return result;
        }

        public Comment createToComment(string postId, CommentCreateModel c, int id = 0)
        {
            Comment result = new Comment(postId, id, c.UserId, c.Text);

            return result;
        }
    }
}
