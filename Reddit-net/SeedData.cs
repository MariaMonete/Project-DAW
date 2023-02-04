using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RedditNet.Models.DatabaseModel;
using RedditNet.UtilityFolder;
using static System.Net.WebRequestMethods;

namespace RedditNet
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider
        serviceProvider)
        {
            using (var context = new AppDbContext(
            serviceProvider.GetRequiredService
            <DbContextOptions<AppDbContext>>()))
            {
                if (context.Roles.Any())
                {
                    return; 
                }

                Hash hash = new Hash();

                context.Roles.AddRange(
                new IdentityRole
                {
                    Id = "2c5e174e-3b0e-446f-86af-483d56fd7210",
                    Name = "Admin",
                    NormalizedName = "Admin".ToUpper()
                },
                new IdentityRole
                {
                    Id = "2c5e174e-3b0e-446f-86af-483d56fd7211",
                    Name = "Moderator",
                    NormalizedName = "Moderator".ToUpper()
                },
                new IdentityRole
                {
                    Id = "2c5e174e-3b0e-446f-86af-483d56fd7212",
                    Name = "Regular",
                    NormalizedName = "Regular".ToUpper()
                }
                );

                var hasher = new PasswordHasher<DatabaseUser>();

                context.Users.AddRange(
                new DatabaseUser
                {
                    Id = hash.sha256_hash("admin"),
                    // primary key
                    UserName = "admin@test.com",
                    EmailConfirmed = true,
                    NormalizedEmail = "ADMIN@TEST.COM",
                    Email = "admin@test.com",
                    NormalizedUserName = "ADMIN@TEST.COM",
                    PasswordHash = hasher.HashPassword(null, "Admin1!")
                },
                new DatabaseUser
                {

                    Id = hash.sha256_hash("moderator"),
                    // primary key
                    UserName = "moderator@test.com",
                    EmailConfirmed = true,
                    NormalizedEmail = "MODERATOR@TEST.COM",
                    Email = "moderator@test.com",
                    NormalizedUserName = "MODERATOR@TEST.COM",
                    PasswordHash = hasher.HashPassword(null, "Moderator1!")
                },
                new DatabaseUser
                {
                    Id = hash.sha256_hash("regular"),
                    // primary key
                    UserName = "regular@test.com",
                    EmailConfirmed = true,
                    NormalizedEmail = "REGULAR@TEST.COM",
                    Email = "regular@test.com",
                    NormalizedUserName = "REGULAR@TEST.COM",
                    PasswordHash = hasher.HashPassword(null,"Regular1!")
                }
                );

                context.UserRoles.AddRange(
                new IdentityUserRole<string>
                {
                    RoleId = "2c5e174e-3b0e-446f-86af-483d56fd7210",
                    UserId = hash.sha256_hash("admin")
                },
                new IdentityUserRole<string>
                {
                    RoleId = "2c5e174e-3b0e-446f-86af-483d56fd7211",
                    UserId = hash.sha256_hash("moderator")
                },
                new IdentityUserRole<string>
                {
                    RoleId = "2c5e174e-3b0e-446f-86af-483d56fd7212",
                    UserId = hash.sha256_hash("regular")
                }
                );
                context.SaveChanges();
            }
        }
    }
}
