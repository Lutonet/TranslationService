using TranslationService.Models;

namespace TranslationService.Services
{
    public interface ITranslationMaker
    {
        Task<List<Models.Translation>> Translate(List<Models.Translation> toAdd, CancellationToken stopToken);
    }
}