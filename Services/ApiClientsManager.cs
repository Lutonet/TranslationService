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
    public class ApiClientsManager
    {
        private ISettingsReader _reader;

        public ApiClientsManager(ISettingsReader reader)
        {
            _reader=reader;
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
            List<ServerTestResult> workingServers = servers.Where(s => s.Successful).OrderBy(s => s.ResponseTime).ToList();
            List<HttpClient> clients = new List<HttpClient>();
            foreach (var ser in workingServers)
            {
                HttpClient cli = new HttpClient();
                cli.BaseAddress = new Uri(ser.Server);

                clients.Add(cli);
            }
            return clients;
        }

        private async Task<List<Language>> GetLanguagesAsync(List<HttpClient> clients)
        {
            int i = 0;
            int count = clients.Count;
            bool success = false;

            while (i < count && success == false)
            {
                try
                {
                    using HttpClient client = clients[i];
                    string json = await client.GetStringAsync("/languages");
                    List<Language> langs = JsonConvert.DeserializeObject<List<Language>>(json);
                    if (langs == null)
                        i++;
                    else
                    {
                        success = true;
                        return langs;
                    }
                }
                catch
                {
                    i++;
                }
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
                    Console.WriteLine("Found " + languages.Count + " languages on " + url);
                    stop = DateTime.Now;
                    Console.WriteLine($"Time: {stop.Millisecond - start.Millisecond}");
                    return new ServerTestResult
                    {
                        Successful = true,
                        Server = url,
                        ResponseTime = stop.Millisecond - start.Millisecond
                    };
                }
                catch (Exception ex)
                {
                    stop = DateTime.Now;
                    return new ServerTestResult
                    {
                        Successful = false,
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
                    Successful = false,
                    Server = url,

                    Error = ex.Message
                };
            }
        }
    }
}