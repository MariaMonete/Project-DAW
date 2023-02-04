using RedditNet.Models.PostModel;
using RedditNet.Models.SubRedditModel;

namespace RedditNet.SubRedditFolder
{
    public class SubRedditMapper
    {
        public SubRedditPostsModel toPostsModel(List<PostPreviewModel> pm, SubReddit s)
        {
            SubRedditPostsModel result = new SubRedditPostsModel();
            result.Posts = pm;
            result.Description = s.Description;
            result.Name = s.Name;
            result.Id = s.Id;

            return result;
        }

        public SubReddit createModelToSubReddit(SubRedditCreateModel m)
        {
            return new SubReddit(m);
        }

        public SubRedditPreviewModel toPreviewModel(SubReddit s)
        {
            SubRedditPreviewModel res = new SubRedditPreviewModel();
            res.Description = s.Description;
            res.Name = s.Name;
            res.Id = s.Id;

            return res;
        }
    }
}
