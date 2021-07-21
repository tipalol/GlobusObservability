using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using GlobusObservability.Core.Entities;
using Newtonsoft.Json;
using Serilog;

namespace GlobusObservability.Rest.Helpers
{
    public class VmPushHelper
    {
        private const string Uri = "http://172.24.217.5:8428/api/v1/import";
        private ILogger _logger;

        public VmPushHelper(ILogger logger)
        {
            _logger = logger;
        }

        public async Task Push(JsonMetricsModel model)
        {
            var client = new HttpClient();
            var counter = 0;
            var status = new List<Task>();
                foreach (var metric in model.Metrics)
                {
                    status.Add(Task.Run(async () =>
                    {
                        counter++;
                    foreach (var values in metric.Value)
                    {
                        foreach (var value in values)
                        {
                            foreach (var measures in values)
                            {
                            var measureCounter = 0;
                                foreach (var measure in measures.Value)
                                {
                                measureCounter++;
                                    var metricId = measure.Key;

                                    var vmModel = new VmModel()
                                    {
                                        metric = new Dictionary<string, string>()
                                        {
                                            {"__name__", measure.Key.Replace("statsfill", "")},
                                            {"instance", "GlobusObservability"},
                                            {"job", "GlobusMetrics"},
                                            {"measureId", metric.Id},
                                            {"nodeName", model.NodeName.Replace("statsfill", "")},
                                            {"nodeInfo", measures.Key}
                                        },
                                        values = measure.Value,
                                        timestamps = new [] {((DateTimeOffset) metric.Duration).ToUnixTimeMilliseconds()}
                                    };

                                    if (model.SubNetworks.Length == 1)
                                        vmModel.metric["subNetwork1"] = model.SubNetworks[0];
                                    
                                    if (model.SubNetworks.Length == 2)
                                        vmModel.metric["subNetwork2"] = model.SubNetworks[1];

                                    if (model.SubNetworks.Length == 3)
                                        vmModel.metric["subNetwork3"] = model.SubNetworks[2];

                                    var dynamicProperties = measures.Key.Split(',');

                                    foreach (var propertyPair in dynamicProperties)
                                    {
                                        var pair = propertyPair.Split('=');

                                        vmModel.metric.Add(pair[0], pair[1]);
                                    }

                                    var response = await client.PostAsync(Uri, new StringContent(JsonConvert.SerializeObject(vmModel)));
                                    _logger.Debug($"Metric {measureCounter}/{measures.Value.Count} from metric {counter}/{model.Metrics.Count} posted to VM. {JsonConvert.SerializeObject(vmModel)}");
                                    _logger.Debug($"Response was {response.StatusCode} {await response.Content.ReadAsStringAsync()}");
                                }
                            }
                        }
                    }
                    }));
                
                }

                Task.WaitAll(status.ToArray(), CancellationToken.None);
        }

        private class VmModel
        {
            public Dictionary<string, string> metric { get; set; }
            public long[] values { get; set; }
            public long[] timestamps { get; set; }
        }

        private class VmMetric
        {
            public string __name__ { get; set; }
            public string job { get; set; }
            public string instance { get; set; }
    
            public string subNetwork1 { get; set; }
            
            public string subNetwork2 { get; set; }
            
            public string subNetwork3 { get; set; }
            public string nodeName { get; set; }
            public string measureId { get; set; }
            public string nodeInfo { get; set; }
        }
    }
}