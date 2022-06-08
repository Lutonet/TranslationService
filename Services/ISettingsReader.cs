using TranslationService.Models;

namespace TranslationService.Services
{
    public interface ISettingsReader
    {
        Task<Settings> ReadSettings();
    }
}