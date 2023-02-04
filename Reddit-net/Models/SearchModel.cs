
using RedditNet.Models.CommentModel;
using RedditNet.Models.PostModel;
using RedditNet.Models.SubRedditModel;
using RedditNet.Models.UserModel;

namespace RedditNet.Models
{
    public class SearchModel
    {
        public IEnumerable<CommentThreadModel> Comments { get; set; }
        public IEnumerable<PostPreviewModel> Posts { get; set; }
        public IEnumerable<SubRedditPreviewModel> Subs { get; set; }
        public IEnumerable<UserShowModel> Users { get; set; }
    }
}
