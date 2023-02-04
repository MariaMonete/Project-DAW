using Microsoft.EntityFrameworkCore;
using RedditNet.PostFolder;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RedditNet.Models.DatabaseModel
{
    [Index(nameof(Id), IsUnique = true)]
    public class DatabasePost
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; }
        public string Title { get; set; }

        [ForeignKey(nameof(DatabaseSubReddit))]
        [Required]
        public string SubId { get; set; }
        //public string UserId { get; set; }
        public string Text { get; set; }
        public int Votes { get; set; }

        public virtual DatabaseUser User { get; set; }
    }
}
