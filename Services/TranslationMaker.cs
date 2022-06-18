using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslationService.Models;

namespace TranslationService.Services
{
    public class TranslationMaker : ITranslationMaker
    {
        private IApiClientsManager _clients;

        public TranslationMaker(IApiClientsManager clients)
        {
            _clients = clients;
        }

        public async Task<List<Models.Translation>> Translate(List<Models.Translation> toAdd,
                                                    CancellationToken stopToken)
        {
            List<HttpClient> clients = await _clients.GetServers();
            int clientsCount = clients.Count;
            if (clientsCount == 0)
                return null;

            // get list of APIs
            // get list of translations
            // separate into API's count amount of lists
            // start tasks
            // collect results and join them together

            return new List<Models.Translation>();
        }
    }
}