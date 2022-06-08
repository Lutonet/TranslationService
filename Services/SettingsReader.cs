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
            try
            {
                return JsonConvert.DeserializeObject<Settings>(await File.ReadAllTextAsync(Path.Combine(Directory.GetCurrentDirectory(), "settings.json")));
            }
            catch (Exception ex)
            {
                _logger.LogError("Can't read settings", ex);
                throw;
            }
        }
    }
}