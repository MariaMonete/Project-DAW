using RedditNet.CommentFolder;
using RedditNet.Models.CommentModel;
using RedditNet.Models.PostModel;
using RedditNet.UtilityFolder;

namespace RedditNet.PostFolder
{
    public class Post : Submission
    {
        private CommentNode root;
        private string id;
        private string title;
        private String subId;

        public void setDeletedState()
        {
            base.setDeletedState();
            Title = Constants.deleted;
        }
        public Post() : base()
        {

        }

        public bool isSame(PostModel p)
        {
            return UserId == p.UserId;
        }

        public void update(PostUpdateModel p)
        {
            Text = p.Text == null ? Text : p.Text;
            Votes = p.Votes == null ? Votes : (int)p.Votes;
        }

        public Post(PostCreateModel p) : this(p.Title, p.UserId, p.Text, p.SubId)
        {

        }

        public Post(string title, string userId, string text, string subId, string id = null, int votes = 0) : base(userId, text, votes)
        {
            Title = title;
            SubId = subId;
            root = new CommentNode(0);
            if (id == null)
            {
                Hash hash = new Hash();

                TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);

                string toHash = "";
                toHash += Convert.ToString((int)t.TotalSeconds);
                toHash += title;
                toHash += text;
                toHash += userId;

                Id = hash.sha256_hash(toHash);
            }
            else
                Id = id;
        }

        public CommentNode Root { get => root; set => root = value; }
        public string Id { get => id; set => id = value; }
        public string Title { get => title; set => title = value; }
        public string SubId { get => subId; set => subId = value; }
    }
}
