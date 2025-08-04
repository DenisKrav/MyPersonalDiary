using Microsoft.AspNetCore.Identity;
using MyPersonalDiary.DAL.Enums;
using MyPersonalDiary.DAL.Models.Identities;

namespace MyPersonalDiary.Server.Seeders
{
    public static class RoleSeeder
    {
        public static async Task SeedRolesAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
            string[] roles = Enum.GetNames(typeof(UserRole));

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new ApplicationRole { Name = role });
            }
        }
    }

}
