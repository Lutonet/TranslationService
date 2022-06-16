using Newtonsoft.Json;
using TranslationService.Models;
using TranslationService.Services;

namespace TranslationService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ISettingsReader _settings;
        private readonly IApiClientsManager _manager;

        public Worker(ILogger<Worker> logger, ISettingsReader settings, IApiClientsManager manager)
        {
            _logger = logger;
            _settings = settings;
            _manager = manager;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // Get Settings
                Settings set = await _settings.ReadSettings();
                // Get Languages
                List<Language> availableLanguages = await _manager.GetLanguagesAsync(await _manager.GetServers());
                List<Language> forTranslation = new List<Language>();
                foreach (Language lang in availableLanguages)
                {
                    if (lang.Code == set.DefaultLanguage || set.IgnoreLanguages.Where(s => s == lang.Code).Any())
                        continue;
                    forTranslation.Add(lang);
                }

                // Check Folders
                List<string> folders = set.TranslationFolders;
                foreach (string folder in folders)
                {
                    string defaultPath = Path.Combine(folder, $"{set.DefaultLanguage}.json");
                    Dictionary<string, string> DefaultTranslation = new Dictionary<string, string>();
                    try
                    {
                        string json = await File.ReadAllTextAsync(defaultPath);
                        DefaultTranslation = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                        if (DefaultTranslation == null)
                            break;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Error opening {defaultPath} ", ex.Message);
                        break;
                    }
                    _logger.LogInformation($"Default Translation found with {DefaultTranslation.Count} phrases");

                    // We are in one of folders now we need to do main job

                    // check if old translation exists
                    Dictionary<string, string> oldDefault;
                    if (File.Exists(Path.Combine(defaultPath, "old.json")))
                    {
                        string json = await File.ReadAllTextAsync(Path.Combine(defaultPath, "old.json"));
                        oldDefault = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                        if (oldDefault == null)
                            oldDefault = new Dictionary<string, string>();
                    }

                    // if so load and compare changes - to add and to remove

                    // get complete list of all translations in all files or empty dictionaries
                    // for each generate full list of changes

                    // stoppable

                    // make changes done

                    // stoppable

                    // write changed files

                    await Task.Delay(60000, stoppingToken);
                }
            }
        }
    }