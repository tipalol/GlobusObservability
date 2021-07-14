using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using GlobusObservability.Core.Entities;
using Newtonsoft.Json;

namespace GlobusObservability.Rest.Helpers
{
    public static class PushGatewayHelper
    {
        private const string Protocol = "http";
        private const string Ip = "172.24.217.5";
        private const string Port = "9091";
        private const string Job = "StoneWorkerForThreeDays";
        private const string Instance = "Clown";

        private static string Endpoint => $"{Ip}:{Port}";
        private static string Uri => $"{Protocol}://{Endpoint}/job/{Job}/instance{Instance}";
        
        private static async Task Push(string labels, string id, int value)
        {
            var query = id + "{" + labels + "} " + value;

            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, Uri);
            
            await _client.PostAsync(Uri, new StringContent(query));
        }
        
        private static readonly HttpClient _client = new HttpClient();


        public static async Task PushMetrics(IEnumerable<JsonMetricsModel> metrics)
        {
            foreach (var metric in metrics)
            {

                var results = new Dictionary<string, int>();

                foreach (var model in metric.Metrics)
                {
                    foreach (var values in model.Value)
                    {
                        foreach (var value in values)
                        {
                            foreach (var measures in values)
                            {
                                foreach (var measure in measures.Value)
                                {
                                    var metricId = measure.Key;
                                    var metricLabels = "label=";

                                    foreach (var ms in measure.Value)
                                    {
                                        var metricValue = ms;

                                        metricLabels += $"\"{metric.Name}\"";
                                        metricLabels += $"label=\"{metric.Date}\"";
                                        metricLabels += $"label=\"{metric.SubNetworks}\"";
                                        metricLabels += $"label=\"{metric.NodeName}\"";
                                        metricLabels += $"label=\"{model.Id}\"";
                                        metricLabels += $"label=\"{measure.Key}\"";

                                        await Push(metricLabels, metricId, metricValue);
                                    }
                                }
                            }
                        }
                    }
                }
                
                //await httpClient.PutAsync(Uri, new StringContent(JsonConvert.SerializeObject(metric)));
            }
        }
    }
}