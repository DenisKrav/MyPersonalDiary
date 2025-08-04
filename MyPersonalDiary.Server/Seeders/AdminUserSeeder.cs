using Microsoft.AspNetCore.Identity;
using MyPersonalDiary.DAL.Models.Identities;

namespace MyPersonalDiary.Server.Seeders
{
    public static class AdminUserSeeder
    {
        public static async Task SeedAdminAsync(IServiceProvider services)
        {
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var config = services.GetRequiredService<IConfiguration>();

            var email = config["Seed:Admin:Email"];
            var password = config["Seed:Admin:Password"];
            var nickname = config["Seed:Admin:NickName"];

            if (await userManager.FindByEmailAsync(email) is null)
            {
                var admin = new ApplicationUser
                {
                    Email = email,
                    UserName = email,
                    NickName = nickname,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(admin, password);
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(admin, "Admin");
            }
        }
    }
}
