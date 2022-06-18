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
        private readonly ITranslationMaker _service;

        public Worker(ILogger<Worker> logger, ISettingsReader settings, IApiClientsManager manager, ITranslationMaker service)
        {
            _logger = logger;
            _settings = settings;
            _manager = manager;
            _service = service;
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
                if (availableLanguages != null)
                {
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
                        Dictionary<string, string> ToUpdate = new Dictionary<string, string>();
                        List<Models.Translation> ToAdd = new List<Models.Translation>();
                        List<Models.Translation> ToRemove = new List<Models.Translation>();

                        string defaultPath = Path.Combine(folder, $"{set.DefaultLanguage}.json");
                        Dictionary<string, string> defaultTranslation = new Dictionary<string, string>();
                        try
                        {
                            string json = await File.ReadAllTextAsync(defaultPath);
                            defaultTranslation = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                            if (defaultTranslation == null)
                                break;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Error opening {defaultPath} ", ex.Message);
                            break;
                        }
                        _logger.LogInformation($"Default Translation found with {defaultTranslation.Count} phrases");

                        // We are in one of folders now we need to do main job

                        // check if old translation exists
                        Dictionary<string, string> oldDefault;
                        if (File.Exists(Path.Combine(defaultPath, "old.json")))
                        {
                            string json = await File.ReadAllTextAsync(Path.Combine(defaultPath, "old.json"));
                            oldDefault = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                            if (oldDefault != null)
                            {
                                // compare old and new translations
                                foreach (var key in defaultTranslation.Keys)
                                {
                                    if (oldDefault.ContainsKey(key))
                                    {
                                        if (oldDefault[key] != defaultTranslation[key])
                                        {
                                            ToUpdate.Add(key, defaultTranslation[key]);
                                            _logger.LogInformation($"Translation for {key} has changed");
                                        }
                                    }
                                }
                            }
                            // happy enough we have now list of updated phrases since last time
                        }

                        // get complete list of all translations in all files or empty dictionaries
                        foreach (Language lang in forTranslation)
                        {
                            string LanguageCode = lang.Code;
                            string langPath = Path.Combine(folder, $"{lang.Code}.json");
                            Dictionary<string, string> translation = new Dictionary<string, string>();
                            try
                            {
                                string json = await File.ReadAllTextAsync(langPath);
                                translation = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                                if (translation == null) translation = new Dictionary<string, string>();
                            }
                            catch
                            {
                                translation = new Dictionary<string, string>();
                            }

                            // check for missing keys
                            foreach (var key in defaultTranslation.Keys)
                            {
                                if (!translation.ContainsKey(key))
                                {
                                    ToAdd.Add(new Models.Translation { Key = key, Value = defaultTranslation[key], LanguageCode = LanguageCode });
                                    //_logger.LogInformation($"Translation for {key} has been added to {lang.Name}");
                                }
                            }

                            // check for redundand keys
                            foreach (var key in translation.Keys)
                            {
                                if (!defaultTranslation.ContainsKey(key))
                                {
                                    ToRemove.Add(new Models.Translation { Key = key, Value = translation[key], LanguageCode = LanguageCode });
                                    //_logger.LogInformation($"Translation for {key} has been removed from {lang.Name}");
                                }
                            }

                            // add updates to all languages

                            foreach (var key in ToUpdate.Keys)
                            {
                                foreach (var lng in forTranslation)
                                {
                                    ToAdd.Add(new Models.Translation { Key = key, Value = ToUpdate[key], LanguageCode = LanguageCode });
                                    //_logger.LogInformation($"Translation for {key} has been updated in {lang.Name}");
                                }
                            }

                            // stoppable
                            // Need to pick three lists and pass them to translation service  - return translations list
                            List<Models.Translation> list = await _service.Translate(ToAdd, stoppingToken);
                            // make changes done

                            // stoppable

                            // write changed files
                        }

                        _logger.LogInformation($"To add: {ToAdd.Count}, to remove: {ToRemove.Count}, to update {ToUpdate.Count}");
                    }
                }
                else
                    _logger.LogInformation("No languages found");
                await Task.Delay(60000, stoppingToken);
            }
        }
    }
}