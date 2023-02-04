using RedditNet.UtilityFolder;

namespace RedditNet
{
    public class Submission
    {
        private String? userId;
        private String text;
        private int votes;

        public void setDeletedState()
        {
            //UserId = Constants.deleted;
            Text = Constants.deleted;
        }
        public Submission()
        {

        }

        public Submission(string userId, string text, int votes = 0)
        {
            UserId = userId;
            Text = text;
            Votes = votes;
        }

        public void update(Submission s)
        {
            Text = s.Text == null ? Text : s.Text;
            Votes = s.Votes == null ? Votes : s.Votes;
        }

        public string UserId { get => userId; set => userId = value; }
        public string Text { get => text; set => text = value; }
        public int Votes { get => votes; set => votes = value; }
    }
}
