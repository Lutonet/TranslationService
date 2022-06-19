using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using TranslationService.Models;

namespace TranslationService.Services
{
    public class ApiClientsManager : IApiClientsManager
    {
        private ISettingsReader _reader;
        private ILogger<ApiClientsManager> _logger;

        public ApiClientsManager(ISettingsReader reader, ILogger<ApiClientsManager> logger)
        {
            _reader=reader;
            _logger = logger;
        }

        public async Task<List<HttpClient>> GetServers()
        {
            List<ServerTestResult> servers = new();
            Settings settings = await _reader.ReadSettings();
            for (int i = 0; i < settings.TranslationServers.Count; i++)
            {
                string server = settings.TranslationServers[i];
                servers.Add(await TestApi(server));
            }
            List<ServerTestResult> workingServers = servers.Where(s => s.Successfull).OrderBy(s => s.ResponseTime).ToList();
            List<HttpClient> clients = new List<HttpClient>();
            foreach (var ser in workingServers)
            {
                HttpClient cli = new HttpClient();
                cli.BaseAddress = new Uri(ser.Server);

                clients.Add(cli);
            }
            return clients;
        }

        public async Task<List<Language>> GetLanguagesAsync(List<HttpClient> clients)
        {
            int i = 0;
            int count = clients.Count;
            try
            {
                using HttpClient client = clients[0];
                string json = await client.GetStringAsync("/languages");
                List<Language> langs = JsonConvert.DeserializeObject<List<Language>>(json);
                if (langs == null)
                    i++;
                else
                {
                    _logger.LogInformation($"Found {langs.Count} languages");
                    return langs;
                }
            }
            catch
            {
                i++;
            }

            return null;
        }

        private async Task<ServerTestResult> TestApi(string url)
        {
            using HttpClient tester = new HttpClient();
            tester.BaseAddress = new Uri(url);
            try
            {
                DateTime start = DateTime.Now;
                DateTime stop;
                string result = await tester.GetStringAsync("/languages");
                try
                {
                    List<Language> languages = JsonConvert.DeserializeObject<List<Language>>(result);
                    stop = DateTime.Now;
                    return new ServerTestResult
                    {
                        Successfull = true,
                        Server = url,
                        ResponseTime = stop.Millisecond - start.Millisecond
                    };
                }
                catch (Exception ex)
                {
                    stop = DateTime.Now;
                    return new ServerTestResult
                    {
                        Successfull = false,
                        Server = url,
                        ResponseTime = stop.Millisecond - start.Millisecond,
                        Error = ex.Message
                    };
                }
            }
            catch (Exception ex)
            {
                return new ServerTestResult
                {
                    Successfull = false,
                    Server = url,

                    Error = ex.Message
                };
            }
        }
    }
}