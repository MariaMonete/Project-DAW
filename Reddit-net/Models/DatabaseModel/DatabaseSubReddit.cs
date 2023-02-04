using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace RedditNet.Models.DatabaseModel
{
    [Index(nameof(Id), IsUnique = true)]
    public class DatabaseSubReddit
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual DatabaseUser User { get; set; }
    }
}
