using TranslationService.Models;

namespace TranslationService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly Settings _settings;

        public Worker(ILogger<Worker> logger, Settings settings)
        {
            _logger = logger;
            _settings = settings;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Starting checks at: {time}", DateTimeOffset.Now);
                _logger.LogInformation("Settings loaded:", _settings);
                await Task.Delay(10000, stoppingToken);
            }
        }
    }
}