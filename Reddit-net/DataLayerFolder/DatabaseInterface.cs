using RedditNet.CommentFolder;
using RedditNet.PostFolder;
using RedditNet.SubRedditFolder;
using RedditNet.UserFolder;
using System.Runtime.Serialization;

namespace RedditNet.DataLayerFolder
{
    public class DatabaseInterface
    {
        public static Dictionary<string, Dictionary<int, CommentNode>> treeNodes = new Dictionary<string, Dictionary<int, CommentNode>>();
        public static Dictionary<string, Dictionary<int, Comment>> comments = new Dictionary<string, Dictionary<int, Comment>>();
        public static Dictionary<string, Dictionary<String, Post>> posts = new Dictionary<string, Dictionary<String, Post>>();
        public static Dictionary<String, SubReddit> subs = new Dictionary<String, SubReddit>();
        public static int idGenerator = 0;
    }
}
