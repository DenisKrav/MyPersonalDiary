using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyPersonalDiary.Server.Seeders;

namespace MyPersonalDiary.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SeederController : ControllerBase
    {
        private readonly IServiceProvider _serviceProvider;

        public SeederController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        [HttpPost("roles")]
        public async Task<IActionResult> SeedRoles()
        {
            using var scope = _serviceProvider.CreateScope();
            await RoleSeeder.SeedRolesAsync(scope.ServiceProvider);
            return Ok("Roles seeded.");
        }

        [HttpPost("admin")]
        public async Task<IActionResult> SeedAdmin()
        {
            using var scope = _serviceProvider.CreateScope();
            await AdminUserSeeder.SeedAdminAsync(scope.ServiceProvider);
            return Ok("Admin user seeded.");
        }

        [HttpPost("run")]
        public async Task<IActionResult> SeedAll()
        {
            using var scope = _serviceProvider.CreateScope();
            var services = scope.ServiceProvider;
            await RoleSeeder.SeedRolesAsync(services);
            await AdminUserSeeder.SeedAdminAsync(services);
            return Ok("All seeders executed.");
        }
    }
}
