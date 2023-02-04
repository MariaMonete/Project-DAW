using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RedditNet.Models.DatabaseModel
{
    [Index(nameof(Id), nameof(PostId), IsUnique = true)]
    public class DatabaseComment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int Parent { get; set; }

        public int Depth { get; set; }

        public string Lineage { get; set; }
        [ForeignKey(nameof(DatabasePost))]
        [Required]
        public string PostId { get; set; }

        //public string UserId { get; set; }

        public string Text { get; set; }

        public int Votes { get; set; }

        public bool Deleted { get; set; }

        public virtual DatabaseUser User { get; set; }

    }
}
