using RedditNet.UtilityFolder;

namespace RedditNet.CommentFolder
{
    public class CommentNode : IComparable<CommentNode>
    {
        private int id;
        private int parent;
        private int depth;
        private string lineage;
        private int votes;

        public int CompareTo(CommentNode other)
        {
            return -this.Id.CompareTo(other.Id);
        }

        private string joinLineage(string l, int id)
        {
            if (l == Constants.lineageSeparator)
                return l + id.ToString();
            return l + Constants.lineageSeparator + id.ToString();
        }

        public void makeChildOf(CommentNode parent)
        {
            Parent = parent.Id;
            Depth = parent.Depth + 1;
            Lineage = joinLineage(parent.Lineage, parent.Id);
        }
        public CommentNode(int votes, int parent = Constants.noParent, int depth = 0, string lineage = "/", int id = 0)
        {
            Id = id;
            Parent = parent;
            Depth = depth;
            Lineage = lineage;
            Votes = votes;
        }

        public CommentNode(int id, CommentNode parent)
        {
            Id = id;
            makeChildOf(parent);
        }

        public int Id { get => id; set => id = value; }
        public int Parent { get => parent; set => parent = value; }
        public int Depth { get => depth; set => depth = value; }
        public string Lineage { get => lineage; set => lineage = value; }
        public int Votes { get => votes; set => votes = value; }
    }
}
