using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RedditNet.CommentFolder;
using RedditNet.Models.CommentModel;
using RedditNet.Models.DatabaseModel;
using RedditNet.Models.PostModel;
using RedditNet.PostFolder;
using RedditNet.SubRedditFolder;
using RedditNet.UserFolder;
using RedditNet.UtilityFolder;

namespace RedditNet.DataLayerFolder
{
    public class DataLayerPosts
    {
        private readonly AppDbContext db;
        public DataLayerPosts(AppDbContext db)
        {
            this.db = db;
        }

        public bool movePost(String subId, String oldSubId, String postId)
        {
            DatabasePost? p = readPost(oldSubId, postId);
            if (p != null)
            {
                try
                {
                    p.SubId = subId;
                    db.SaveChanges();
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }

            return false;
        }
        public (List<PostPreviewModel>?, int) getPosts(String search = "", int start = 0)
        {
            if (search != "")
            {
                try
                {
                    var dbp = (from b in db.Posts
                               where (b.Text.Contains(search)) || (b.Title.Contains(search))
                               select b).Include("User").OrderBy(o => o.Id);

                    List<DatabasePost>? posts = dbp.Skip(start * Constants.pageSizePosts).Take(Constants.pageSizePosts).ToList<DatabasePost>();

                    int cnt = dbp.Count();

                    List<PostPreviewModel>? result = new List<PostPreviewModel>();

                    DatabaseMapper mapper = new DatabaseMapper();
                    foreach (DatabasePost p in posts)
                    {
                        PostMapper pm = new PostMapper();
                        result.Add(pm.toPreviewModel(p.User?.UserName ?? "[deleted]",
                            mapper.toPost(p)));
                    }

                    return (result, cnt);

                }
                catch (Exception)
                {

                }
            }

            return (null, 0);
        }

        //private bool hasPermission(User affectedUser, User requestingUser)
        //{
        //    return (affectedUser?.isSame(requestingUser) ?? false) || requestingUser.isAdmin() || requestingUser.isMod();
        //}
        public DatabasePost? createPost(Post post, DatabaseUser user)
        {
            DataLayerComments d = new DataLayerComments(db);
            if (d.createComment(post.Root, new Comment(post.Id, post.Root.Id, post.UserId, "root comment"), user) != null)
            {
                DatabaseMapper mapper = new DatabaseMapper();
                DatabasePost p = mapper.toDBPost(post, user);

                try
                {
                    db.Add<DatabasePost>(p);
                    db.SaveChanges();

                    return p;
                }catch (Exception)
                {
                    return null;
                }
            }
            return null;
        }

        public bool deletePost(string subId, string id, PostDeleteModel p)
        {
            DatabasePost? dbp = readPost(subId, id);
            if (dbp != null)
            {
                try
                {
                    List<DatabaseComment>? comments = (from b in db.Comments
                                                       where (b.PostId == id)
                                                       select b).ToList<DatabaseComment>();
                    foreach (DatabaseComment comment in comments)
                    {
                        db.Remove<DatabaseComment>(comment);
                    }

                    db.SaveChanges();
                }
                catch(Exception)
                {

                }
                try
                {
                    db.Remove<DatabasePost>(dbp);
                    db.SaveChanges();

                    return true;
                }
                catch (Exception)
                {
                    return false;
                }

            }

            return false;
        }

        public DatabasePost? updatePost(string subId, string id, PostUpdateModel p)
        {
            DatabasePost? dbp = readPost(subId, id);
            if (dbp != null)
            {
                try
                {
                    dbp.Text = p.Text ;
                    dbp.Votes = p.Votes;
                    db.SaveChanges();

                    return dbp;
                }
                catch (Exception)
                {
                    return null;
                }

            }

            return null;
        }

        public DatabasePost? readPost(string subId, string id)
        {
            try
            {
                DatabasePost? p = (from b in db.Posts
                                   where (b.Id == id) && (b.SubId == subId)
                                   select b).FirstOrDefault<DatabasePost>();

                return p;
            }catch (Exception)
            {
                return null;
            }

        }



        //
        /*public async Task<DatabasePost> ReadPost(string subId, string id)
        {
            var post = await db.Posts.FirstOrDefaultAsync(x=>x.Id == id &&x.SubId==subId);

            return post is null ? new DatabasePost { } : post;

        }*/
        //

        //
        /*public async Task<DatabasePost> UpdatePost(string subId, string id, PostUpdateModel p)
        {
            var editPost=await ReadPost(subId, id);

            if(editPost != null)
            {
                editPost.Text = string.IsNullOrEmpty(p.Text) ? editPost.Text : p.Text;
                editPost.Votes = p.Votes;

                await db.SaveChangesAsync() ;

                return editPost;
            }

            return new();
        }*/

        //

        //public void removePost(string subId, string id)
        //{
        //    if (DatabaseInterface.posts.ContainsKey(subId) && DatabaseInterface.posts[subId].ContainsKey(id))
        //    {
        //        DatabaseInterface.posts[subId].Remove(id);
        //    }
        //}

        //public void changeSubReddit(String oldSubId, String newSubId, String id, String userId)
        //{
        //    User u = DatabaseInterface.dataLayerUsers.readUser(userId);
        //    if (u != null && 
        //        (u.isMod() || u.isAdmin())
        //        )
        //    {
        //        SubReddit oldSub = DatabaseInterface.dataLayerSubReddits.readSubReddit(oldSubId);
        //        SubReddit newSub = DatabaseInterface.dataLayerSubReddits.readSubReddit(newSubId);

        //        if (oldSub != null && newSub != null)
        //        {
        //            Post p = readPost(oldSub.Id, id);
        //            if (p != null)
        //            {
        //                p.SubId = newSub.Id;
        //                removePost(oldSub.Id, id);
        //                DatabaseInterface.posts[newSubId][id] = p;
        //            }
        //        }
        //    }
        //}
    }
}
