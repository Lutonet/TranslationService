using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslationService.Models;

namespace TranslationService.Services
{
    public class SettingsReader : ISettingsReader
    {
        private ILogger<SettingsReader> _logger;

        public SettingsReader(ILogger<SettingsReader> logger)
        {
            _logger = logger;
        }

        public async Task<Settings> ReadSettings()
        {
            string json;
            string path = Path.Combine(Directory.GetCurrentDirectory(), "settings.json");
            try
            {
                json = await File.ReadAllTextAsync(path);
                Settings settings = JsonConvert.DeserializeObject<Settings>(json);
                if (settings == null)
                {
                    _logger.LogError("Settings are empty, can't continue");
                    throw new Exception("Settings are empty");
                }
                _logger.LogInformation("Settings loaded and readed successfully");
                return settings;
            }
            catch (Exception ex)
            {
                _logger.LogError("Can't read settings", ex);
                throw;
            }
        }
    }
}