using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RedditNet.Models.DatabaseModel;

namespace RedditNet
{
    public class AppDbContext : IdentityDbContext<DatabaseUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext>
        options)
        : base(options)
        {
        }

        public DbSet<DatabaseComment> Comments { get; set; }
        public DbSet<DatabasePost> Posts { get; set; }
        public DbSet<DatabaseSubReddit> SubReddits { get; set; }
    }
}
