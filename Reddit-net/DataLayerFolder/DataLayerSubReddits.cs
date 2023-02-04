
using RedditNet.Models.DatabaseModel;
using RedditNet.Models.PostModel;
using RedditNet.Models.SubRedditModel;
using RedditNet.PostFolder;
using RedditNet.SubRedditFolder;
using RedditNet.UserFolder;
using RedditNet.UtilityFolder;
using System;

namespace RedditNet.DataLayerFolder
{
    public class DataLayerSubReddits
    {
        private readonly AppDbContext db;
        public DataLayerSubReddits(AppDbContext db)
        {
            this.db = db;
        }
        //private bool hasPermission(String userId)
        //{
        //    User u = DatabaseInterface.dataLayerUsers.readUser(userId);
        //    if (u == null)
        //        return false;

        //    return u.isAdmin();
        //}
        public DatabaseSubReddit? createSubReddit(SubReddit s, String userId)
        {
            DatabaseMapper mapper = new DatabaseMapper();
            DatabaseSubReddit subReddit = mapper.toDBSubReddit(s);

            try
            {
                db.Add<DatabaseSubReddit>(subReddit);
                db.SaveChanges();

                return subReddit;
            }catch (Exception)
            {

            }

            return null;
        }

        public bool deleteSubReddit(String id, String userId)
        {
            DatabaseSubReddit? dbs = readSubReddit(id);
            if (dbs != null)
            {
                try
                {
                    List<DatabasePost>? posts = (from b in db.Posts
                                                 where (b.SubId == dbs.Id)
                                                 select b).ToList<DatabasePost>();
                    DataLayerPosts dp = new DataLayerPosts(db);
                    foreach (DatabasePost post in posts)
                    {
                        dp.deletePost(post.SubId, post.Id, new PostDeleteModel());
                    }

                }
                catch (Exception)
                {

                }
                try
                {
                    db.Remove<DatabaseSubReddit>(dbs);
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

        public DatabaseSubReddit? readSubReddit(String id)
        {
            try
            {
                DatabaseSubReddit? p = (from b in db.SubReddits
                                   where (b.Id == id)
                                   select b).FirstOrDefault<DatabaseSubReddit>();

                return p;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public DatabaseSubReddit? updateSubReddit(String id, SubRedditUpdateModel m)
        {
            DatabaseSubReddit? dbp = readSubReddit(id);
            if (dbp != null)
            {
                try
                {
                    dbp.Description = m.Description;
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

        public (List<PostPreviewModel>?, int) getPosts(String id, int start)
        {
            List<PostPreviewModel> result = new List<PostPreviewModel>();
            var posts = (from b in db.Posts
                         where (b.SubId == id)
                         select b).OrderBy(o => o.Id);

            int cnt = posts.Count();
            List<DatabasePost>? dbPosts = posts.Skip(start * Constants.pageSizePosts).Take(Constants.pageSizePosts).ToList<DatabasePost>();

            if (dbPosts == null)
                return (null, 0);

            PostMapper mapper = new PostMapper();
            DatabaseMapper dmapper = new DatabaseMapper();
            foreach (DatabasePost x in dbPosts)
            {
                //String userName = DatabaseInterface.dataLayerUsers.readUser(p.UserId).UserName;
                Post post = dmapper.toPost(x);
                DatabaseUser? u = null;
                try
                {
                    u = (from b in db.Users where post.UserId == b.Id select b).FirstOrDefault();
                }catch(Exception)
                {

                }

                String userName = "[deleted]";
                if (u != null)
                {
                    userName = u.UserName;
                }

                PostPreviewModel pm = mapper.toPreviewModel(userName, post);
                result.Add(pm);
            }

            return (result, cnt);
        }

        public (List<SubRedditPreviewModel>?, int) getSubs(String search = "", int start = 0)
        {
            List<SubRedditPreviewModel> result = new List<SubRedditPreviewModel>();
            List<DatabaseSubReddit>? dbSubs = null;

            int cnt = 0;
            if (search == "")
            {
                dbSubs = (from b in db.SubReddits
                                                   where (1 == 1)
                                                   select b).ToList<DatabaseSubReddit>();
                cnt = dbSubs.Count();
            }else
            {
                var ds = (from b in db.SubReddits
                          where (1 == 1) && (b.Description.Contains(search)) || (b.Name.Contains(search))
                          select b).OrderBy(o => o.Id);

                dbSubs = ds.Skip(start * Constants.pageSizePosts).Take(Constants.pageSizePosts).ToList<DatabaseSubReddit>();
                cnt = ds.Count();
            }


            if (dbSubs == null)
                return (null, 0);

            SubRedditMapper mapper = new SubRedditMapper();
            DatabaseMapper dmapper = new DatabaseMapper();
            foreach (DatabaseSubReddit x in dbSubs)
            {
                //String userName = DatabaseInterface.dataLayerUsers.readUser(p.UserId).UserName;
                SubReddit sub = dmapper.toSubReddit(x);
                SubRedditPreviewModel pm = mapper.toPreviewModel(sub);
                result.Add(pm);
            }

            return (result, cnt);
        }
    }
}
