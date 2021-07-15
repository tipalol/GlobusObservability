using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using GlobusObservability.Core.Entities;
using Newtonsoft.Json;
using Serilog;
using Prometheus;

namespace GlobusObservability.Rest.Helpers
{
    public class PushGatewayHelper
    {
        private const string Protocol = "http";
        private const string Ip = "172.24.217.5";
        private const string Port = "9091";
        private const string Job = "StoneWorkerForThreeDays";
        private const string Instance = "Clown";

        private readonly ILogger _logger;
        
        private static string Endpoint => $"{Ip}:{Port}";
        private static string Uri => $"{Protocol}://{Endpoint}/job/{Job}/instance/{Instance}";

        public PushGatewayHelper(ILogger logger)
        {
            _logger = logger;
        }
        
        private async Task Push(string labels, string id, int value)
        {
            var query = " " + id + "{" + labels + "} " + value;

            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, Uri);
            
            _logger.Debug(Uri + query);
            
            await _client.PostAsync(Uri + " " + query, new StringContent(""));
        }
        
        private readonly HttpClient _client = new HttpClient();


        public async Task PushMetrics(IEnumerable<JsonMetricsModel> metrics)
        {
            var pusher = new MetricPusher(new MetricPusherOptions()
            {
                Endpoint = $"{Protocol}://{Ip}:{Port}/metrics/",
                Job = "GlobusMetrics",
                Instance = "GlobusObservability"
            });
            pusher.Start();
            
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
                                    var metricLabels = "name=";

                                    foreach (var ms in measure.Value)
                                    {
                                        var metricValue = ms;

                                        metricLabels += $"\"{metric.Name}\"";
                                        metricLabels += $"date=\"{metric.Date}\"";
                                        metricLabels += $"subNetworks=\"{metric.SubNetworks}\"";
                                        metricLabels += $"nodeName=\"{metric.NodeName}\"";
                                        metricLabels += $"id=\"{model.Id}\"";
                                        metricLabels += $"measureId=\"{measure.Key}\"";

                                        var networks = metric.SubNetworks.Aggregate("", (current, net) => current + (net + "-"));

                                        var promMetric = Metrics.CreateGauge(measure.Key, "", new []
                                        {
                                            $"{metric.Date:yyyy-MM-dd-HH-mm-ss}-{metric.NodeName}-{networks}",
                                            $"{metric.Date:yyyy-MM-dd-HH-mm-ss}",
                                            $"{networks}",
                                            $"{metric.NodeName}",
                                            $"{model.Id}",
                                            $"{measure.Key}"
                                        });
                                        promMetric.Set(metricValue);
                                        promMetric.Publish();

                                        //await Push(metricLabels, metricId, metricValue);
                                    }
                                }
                            }
                        }
                    }
                }

                await Task.Delay(10000);

                await pusher.StopAsync();
                //await httpClient.PutAsync(Uri, new StringContent(JsonConvert.SerializeObject(metric)));
            }
        }
    }
}