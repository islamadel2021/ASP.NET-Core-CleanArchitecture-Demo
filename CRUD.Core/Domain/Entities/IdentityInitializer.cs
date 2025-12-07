using Entities.IdentityEntities;
using Microsoft.AspNetCore.Identity;

namespace Entities
{
    public class IdentityInitializer
    {
        public static async Task Initialize(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            if (!userManager.Users.Any())
            {
                //Adding Roles
                var roles = new List<ApplicationRole>
            {
                new ApplicationRole
                {
                    Id = Guid.NewGuid(),
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                new ApplicationRole
                {
                    Id = Guid.NewGuid(),
                    Name = "Moderator",
                    NormalizedName = "MODERATOR"
                },
                new ApplicationRole
                {
                    Id = Guid.NewGuid(),
                    Name = "User",
                    NormalizedName = "USER"
                }
                    };
                for (int i = 0; i < roles.Count; i++)
                {
                    await roleManager.CreateAsync(roles[i]);
                }

                // Add admin user
                var adminUser = new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    PersonName = "Muhammad",
                    UserName = "admin",
                    NormalizedUserName = "ADMIN",
                    Email = "admin@crud.com",
                    NormalizedEmail = "ADMIN@CRUD.COM",
                    PhoneNumber = "1234567890",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(adminUser, "Password1!");
                await userManager.AddToRolesAsync(adminUser, new[] { "User", "Admin" });
            }
        }
    }
}
