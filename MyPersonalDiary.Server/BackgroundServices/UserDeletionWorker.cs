using MyPersonalDiary.BLL.InterfacesServices;

namespace MyPersonalDiary.Server.BackgroundServices
{
    public class UserDeletionWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<UserDeletionWorker> _logger;

        public UserDeletionWorker(IServiceProvider serviceProvider, ILogger<UserDeletionWorker> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("User deletion worker started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

                    var deleted = await userService.PurgeSoftDeletedUsersAsync(TimeSpan.FromDays(2));
                    _logger.LogInformation("User purge completed. Deleted: {Count}", deleted);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during user purge cycle");
                }

                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }

            _logger.LogInformation("User deletion worker stopped.");
        }
    }
}
