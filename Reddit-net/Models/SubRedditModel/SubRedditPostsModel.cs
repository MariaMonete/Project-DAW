using RedditNet.Models.PostModel;

namespace RedditNet.Models.SubRedditModel
{
    public class SubRedditPostsModel
    {
        public List<PostPreviewModel> Posts { get; set; }
        public String Name { get; set; }
        public String Id { get; set; }

        public String Description { get; set; }
    }
}
