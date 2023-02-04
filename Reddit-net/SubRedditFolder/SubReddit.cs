using RedditNet.Models.SubRedditModel;
using RedditNet.UtilityFolder;

namespace RedditNet.SubRedditFolder
{
    public class SubReddit
    {
        private String name;
        private String id;
        private String description;

        public string Name { get => name; set => name = value; }
        public string Id { get => id; set => id = value; }
        public string Description { get => description; set => description = value; }

        public void update(SubRedditUpdateModel m)
        {
            Description = m.Description == null ? Description : m.Description;
        }


        public SubReddit(SubRedditCreateModel m) : this(m.Name, m.Description)
        {

        }

        public SubReddit(string name, string description, string id = null)
        {
            Name = name;
            Description = description;

            if (id == null)
            {
                Hash hash = new Hash();

                Id = hash.sha256_hash(Name);
            }
            else
                Id = id;
        }
    }
}
