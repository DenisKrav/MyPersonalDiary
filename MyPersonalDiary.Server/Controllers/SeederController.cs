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
