using RedditNet.CommentFolder;
using RedditNet.Models.CommentModel;
using RedditNet.PostFolder;
using RedditNet.SubRedditFolder;

namespace RedditNet.Models.DatabaseModel
{
    public class DatabaseMapper
    {
        public DatabaseComment toDBComment(CommentNode n, Comment c, DatabaseUser user)
        {
            DatabaseComment comment = new DatabaseComment();
            comment.Id = 0;
            comment.PostId = c.PostId;
            comment.Depth = n.Depth;
            comment.Parent = n.Parent;
            comment.Votes = c.Votes;
            comment.Text = c.Text;
            comment.User = user;
            comment.Lineage = n.Lineage;

            return comment;
        }

        public CommentThreadModel toThreadComment(DatabaseComment c, String subId)
        {
            CommentMapper mapper = new CommentMapper();
            Comment comment = new Comment(c.PostId, c.Id, c.User?.Id ?? null, c.Text, c.Votes);

            return mapper.toThreadModel(comment, subId, c.Deleted, c.Depth, c.User?.UserName ?? "[deleted]");
        }

        public DatabasePost toDBPost(Post p, DatabaseUser user)
        {
            DatabasePost res = new DatabasePost();
            res.Text = p.Text;
            res.Title = p.Title;
            res.Votes = p.Votes;
            res.Id = p.Id;
            res.User = user;
            res.SubId = p.SubId;

            return res;
        }

        public Post toPost(DatabasePost p)
        {
            Post res = new Post(p.Title, p.User?.Id ?? null, p.Text, p.SubId, p.Id, p.Votes);

            return res;
        }

        public DatabaseSubReddit toDBSubReddit(SubReddit s)
        {
            DatabaseSubReddit res = new DatabaseSubReddit();
            res.Id = s.Id;
            res.Name = s.Name;
            res.Description = s.Description;

            return res;
        }

        public SubReddit toSubReddit(DatabaseSubReddit s)
        {
            SubReddit res = new SubReddit(s.Name, s.Description, s.Id);

            return res;
        }

    }
}
