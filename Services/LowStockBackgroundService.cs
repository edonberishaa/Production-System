// Services/LowStockBackgroundService.cs
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CakeProduction.Services
{
    public class LowStockBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<LowStockBackgroundService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(6);

        public LowStockBackgroundService(
            IServiceProvider services,
            ILogger<LowStockBackgroundService> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Low Stock Background Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _services.CreateScope())
                    {
                        var alertService = scope.ServiceProvider
                            .GetRequiredService<ILowStockAlertService>();
                        await alertService.CheckLowStockLevels();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in Low Stock Background Service");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }

            _logger.LogInformation("Low Stock Background Service is stopping.");
        }
    }
}