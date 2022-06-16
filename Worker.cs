using TranslationService.Models;
using TranslationService.Services;

namespace TranslationService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ISettingsReader _settings;

        public Worker(ILogger<Worker> logger, ISettingsReader settings)
        {
            _logger = logger;
            _settings = settings;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Settings set = await _settings.ReadSettings();
                _logger.LogInformation($"langs: {set.IgnoreLanguages}", set.IgnoreLanguages);
                _logger.LogInformation("Starting checks at: {time}", DateTimeOffset.Now);
                _logger.LogInformation($"Settings loaded: {set}", set);
                _logger.LogInformation($"Default language: {set.DefaultLanguage}", set.DefaultLanguage);

                await Task.Delay(10000, stoppingToken);
            }
        }
    }
}