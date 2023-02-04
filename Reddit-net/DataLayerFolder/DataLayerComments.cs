
using RedditNet.CommentFolder;
using RedditNet.Models.CommentModel;
using RedditNet.Models.DatabaseModel;
using RedditNet.PostFolder;
using RedditNet.UserFolder;
using RedditNet.UtilityFolder;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data.Common;
using System.Security.Cryptography.Pkcs;

namespace RedditNet.DataLayerFolder
{

    public class DataLayerComments
    {
        private readonly AppDbContext db;
        public DataLayerComments(AppDbContext db)
        {
            this.db = db;
        }
        
        public (List<CommentThreadModel>?, int) getComments(String search, int start)
        {
            if (search != "")
            {
                try
                {
                    var dbc = (from b in db.Comments
                               where ((b.Text.Contains(search) && (b.Parent != -1) && b.Deleted == false))
                               select b).OrderBy(o => o.Id);

                    List<DatabaseComment>? dbComments = dbc.Skip(start * Constants.pageSizePosts).Take(Constants.pageSizePosts).ToList<DatabaseComment>();
                    int cnt = dbc.Count();

                    List<CommentThreadModel> result = new List<CommentThreadModel>();
                    DatabaseMapper mapper = new DatabaseMapper();

                    foreach (var x in dbComments)
                    {
                        String? subId = null;
                        try
                        {
                            var post = (from b in db.Posts
                                        where (b.Id == x.PostId)
                                        select b).FirstOrDefault();
                            if (post != null)
                                subId = post.SubId;
                        }
                        catch (Exception)
                        {
                            continue;
                        }

                        if (subId != null)
                        {
                            result.Add(mapper.toThreadComment(x, subId));
                        }
                    }
                    return (result, cnt);
                }
                catch (Exception)
                {
                    return (null, 0);
                }
            }

            return (null, 0);
        }

        //private bool hasPermission(User affectedUser, User requestingUser)
        //{
        //    return (affectedUser?.isSame(requestingUser) ?? false) || requestingUser.isAdmin() || requestingUser.isMod();
        //}
        public DatabaseComment? createComment(CommentNode node, Comment comment, DatabaseUser user)
        {
            if (node.Parent != Constants.noParent)
            {
                DatabaseComment? parent = readComment(comment.PostId, node.Parent);
                if (parent != null)
                {
                    node.makeChildOf(new CommentNode(parent.Votes, parent.Parent, parent.Depth, parent.Lineage, parent.Id));
                }
                else
                    return null;
            }

            DatabaseMapper mapper = new DatabaseMapper();
            DatabaseComment c = mapper.toDBComment(node, comment, user);
            try
            {
                db.Add<DatabaseComment>(c);
                db.SaveChanges();

                return c;
            }catch(Exception)
            {
                return null;
            }
        }

        public bool deleteComment(string postId, int id, CommentDeleteModel c)
        {
            DatabaseComment? dbc = readComment(postId, id);
            if (dbc != null)
            {
                try
                {
                    //db.Remove<DatabaseComment>(dbc);
                    dbc.Deleted = true;
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

        public DatabaseComment? updateComment(string postId, int id, CommentUpdateModel c)
        {
            DatabaseComment? dbc = readComment(postId, id);
            if (dbc != null)
            { 
                try
                {
                    dbc.Text = c.Text;
                    dbc.Votes = c.Votes;
                    db.SaveChanges();

                    return dbc;
                }
                catch (Exception)
                {
                    return null;
                }

            }

            return null;
        }

        //public CommentNode readNode(string postId, int id)
        //{
        //    if (DatabaseInterface.treeNodes.ContainsKey(postId) && DatabaseInterface.treeNodes[postId].ContainsKey(id))
        //    {
        //        return DatabaseInterface.treeNodes[postId][id];
        //    }

        //    return null;
        //}

        public DatabaseComment? readComment(string postId, int id)
        {
            try
            {
                DatabaseComment? dbComment = (from b in db.Comments
                                              where (b.PostId == postId) && (b.Id == id)
                                              select b).FirstOrDefault<DatabaseComment>();
                return dbComment;
            }
            catch(Exception)
            {
                return null;
            } 
        }

        public DatabaseComment? getPostParent(string postId)
        {
            try
            {
                DatabaseComment? dbComment = (from b in db.Comments
                                              where (b.PostId == postId) && (b.Parent == Constants.noParent)
                                              select b).FirstOrDefault<DatabaseComment>();
                return dbComment;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<CommentNode>? getDescendants(string postId, int parentId = Constants.noParent, int cmpMethod = Constants.comparisonByTimeDesc)
        {
            DatabaseComment? parent = parentId == Constants.noParent ? getPostParent(postId) : readComment(postId, parentId);

            if (parent == null)
                return null;

            string prefix = parent.Lineage;

            List<DatabaseComment> dbComms = (from b in db.Comments
                                       where (b.PostId == postId) && (b.Lineage.StartsWith(prefix))
                                       select b).ToList<DatabaseComment>();

            List<CommentNode> nodes = new List<CommentNode>();
            foreach (DatabaseComment x in dbComms)
            {
                nodes.Add(new CommentNode(x.Votes, x.Parent, x.Depth, x.Lineage, x.Id));
            }

            Node root = BuildTreeAndGetRoots(nodes, cmpMethod).First();

            nodes = getDFS(root);

            return nodes;
        }

        private List<CommentNode> getDFS(Node root)
        {
            List<CommentNode> dfs = new List<CommentNode>();

            Stack<Node> s = new Stack<Node>();

            HashSet<Node> visited = new HashSet<Node>();

            s.Push(root);

            while (s.Count > 0)
            {
                Node current = s.Pop();

                if (!visited.Contains(current))
                    dfs.Add(current.commentNode);

                visited.Add(current);
                foreach (Node c in current.Children)
                    if (!visited.Contains(c))
                        s.Push(c);
            }

            return dfs;
        }

        class Node : IComparable<Node>
        {
            public int CompareTo(Node other)
            {
                return this.commentNode.CompareTo(other.commentNode);
            }
            public List<Node> Children = new List<Node>();
            public Node Parent { get; set; }
            public CommentNode commentNode { get; set; }
        }

        IEnumerable<Node> BuildTreeAndGetRoots(List<CommentNode> actualObjects, int cmpMethod)
        {
            Dictionary<int, Node> lookup = new Dictionary<int, Node>();
            actualObjects.ForEach(x => lookup.Add(x.Id, new Node { commentNode = x }));
            foreach (var item in lookup.Values)
            {
                Node proposedParent;
                if (lookup.TryGetValue((int)item.commentNode.Parent, out proposedParent))
                {
                    item.Parent = proposedParent;
                    proposedParent.Children.Add(item);
                }
            }

            foreach (var x in lookup.Values)
            {
                if (x.Parent == null)
                {
                    switch (cmpMethod)
                    {
                        case Constants.comparisonByTimeAsc:
                            x.Children.Sort(byTimeAsc);
                            break;
                        case Constants.comparisonByTimeDesc:
                            x.Children.Sort(byTimeDesc);
                            break;
                        case Constants.comparisonByVotesAsc:
                            x.Children.Sort(byVotesAsc);
                            break;
                        case Constants.comparisonByVotesDesc:
                            x.Children.Sort(byVotesDesc);
                            break;
                        default:
                            x.Children.Sort(byTimeDesc);
                            break;
                    }
                }
                else
                    x.Children.Sort(byTimeDesc);
            }

            return lookup.Values.Where(x => x.Parent == null);
        }

        static Comparison<Node> byTimeDesc = delegate (Node object1, Node object2)
        {
            return object1.commentNode.CompareTo(object2.commentNode);
        };

        static Comparison<Node> byVotesDesc = delegate (Node object1, Node object2)
        {
            return object1.commentNode.Votes.CompareTo(object2.commentNode.Votes);
        };

        static Comparison<Node> byTimeAsc = delegate (Node object1, Node object2)
        {
            return -object1.commentNode.CompareTo(object2.commentNode);
        };

        static Comparison<Node> byVotesAsc = delegate (Node object1, Node object2)
        {
            return -object1.commentNode.Votes.CompareTo(object2.commentNode.Votes);
        };

    }
}
