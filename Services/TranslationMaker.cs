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
        private ILogger<TranslationMaker> _logger;

        public TranslationMaker(IApiClientsManager clients, ILogger<TranslationMaker> logger)
        {
            _clients = clients;
            _logger = logger;
        }

        public async Task<List<Translation>> Translate(List<Models.Translation> toAdd,
                                                    CancellationToken stopToken)
        {
            List<HttpClient> clients = await _clients.GetServers();
            int clientsCount = clients.Count;
            if (clientsCount == 0)
                return null;

            // create queue List
            List<List<Translation>> queue = new();
            for (int k = 0; k < clientsCount; k++)
            {
                queue.Add(new List<Translation>());
            }

            // create lists for each client
            Console.WriteLine($"Sorting {toAdd.Count} to {clientsCount} workers");
            for (int i = 0; i < toAdd.Count; i++)
            {
                int index = i % clientsCount;
                queue[index].Add(toAdd[i]);
            }

            //Initialize clients
            List<Task> task = new List<Task>();
            _logger.LogInformation($"Clients: {clients.Count}");
            for (int j = 0; j < clientsCount; j++)
            {
                task.Add(new Task(() => GetTranslations(queue[j], stopToken, j)));
                _logger.LogInformation($"Starting worker {j} with {queue[j].Count} requests");
                task[j].Start();
                Task.Delay(50).Wait();
            }

            // collect results and join them together
            Task.WaitAll(task.ToArray());
            Console.WriteLine("Tasks finished");
            return new List<Models.Translation>();
        }

        public List<Translation> GetTranslations(List<Translation> toTranslate, CancellationToken stopToken, int index)
        {
            _logger.LogInformation($"Starting translate {toTranslate.Count} phrases on worker {index}");
            // configure HTTP client

            // cycle through phrases and get translations - if not stopToken activated

            // error check - return error List
            return new List<Translation>();
        }
    }
}