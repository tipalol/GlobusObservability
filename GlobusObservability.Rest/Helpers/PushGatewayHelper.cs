using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using GlobusObservability.Core.Entities;
using Newtonsoft.Json;

namespace GlobusObservability.Rest.Helpers
{
    public static class PushGatewayHelper
    {
        private const string Ip = "172.24.217.5";
        private const string Port = "9091";
        private const string Job = "StoneWorkerForThreeDays";
        private const string Instance = "Clown";

        private static string Endpoint => $"{Ip}:{Port}";
        private static string Uri => $"{Endpoint}/job/{Job}/instance{Instance}";

        public static async Task PushMetrics(IEnumerable<JsonMetricsModel> metrics)
        {
            var httpClient = new HttpClient();

            foreach (var metric in metrics)
                await httpClient.PutAsync(Uri, new StringContent(JsonConvert.SerializeObject(metric)));
        }
    }
}