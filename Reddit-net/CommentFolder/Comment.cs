using RedditNet.Models.CommentModel;
using System.Data;

namespace RedditNet.CommentFolder
{
    public class Comment : Submission
    {
        private string postId;
        private int id;

        public void setDeletedState()
        {
            base.setDeletedState();
        }
        public Comment(string postId, int id) : base()
        {
            PostId = postId;
            Id = id;
        }

        public Comment(string postId, int id, string userId, string text, int votes = 0) : base(userId, text, votes)
        {
            PostId = postId;
            Id = id;
        }

        public bool isSame(CommentModel c)
        {
            return UserId == c.UserId;
        }

        public void update(CommentUpdateModel c)
        {
            Text = c.Text == null ? Text : c.Text;
            Votes = c.Votes == null ? Votes : (int)c.Votes;
        }

        public string PostId { get => postId; set => postId = value; }
        public int Id { get => id; set => id = value; }
    }
}
