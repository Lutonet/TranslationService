using TranslationService.Models;

namespace TranslationService.Services
{
    public interface IApiClientsManager
    {
        Task<List<Language>> GetLanguagesAsync(List<HttpClient> clients);
        Task<List<HttpClient>> GetServers();
    }
}