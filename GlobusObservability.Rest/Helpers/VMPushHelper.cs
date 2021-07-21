using System;
using System.Collections.Generic;
using System.Net.Http;
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

                foreach (var metric in model.Metrics)
                {
                    foreach (var values in metric.Value)
                    {
                        foreach (var value in values)
                        {
                            foreach (var measures in values)
                            {
                                foreach (var measure in measures.Value)
                                {
                                    var metricId = measure.Key;
                                    
                                        var vmModel = new VmModel()
                                    {
                                        metric = new VmMetric()
                                        {
                                            __name__ = measure.Key,
                                            instance = "GlobusObservability",
                                            job = "GlobusMetrics",
                                            measureId = metric.Id,
                                            nodeName = model.NodeName,
                                            subNetoworks = model.SubNetworks,
                                            nodeInfo = measures.Key
                                        },
                                        values = measure.Value,
                                        timestamps = new [] {metric.Duration}
                                    };

                                        await client.PostAsync(Uri, new StringContent(JsonConvert.SerializeObject(vmModel)));
                                        _logger.Debug($"Metric posted to VM. {JsonConvert.SerializeObject(vmModel)}");
                                }
                            }
                        }
                    }
                }
        }

        private class VmModel
        {
            public VmMetric metric { get; set; }
            public long[] values { get; set; }
            public DateTime[] timestamps { get; set; }
        }

        private class VmMetric
        {
            public string __name__ { get; set; }
            public string job { get; set; }
            public string instance { get; set; }
            public string[] subNetoworks { get; set; }
            public string nodeName { get; set; }
            public string measureId { get; set; }
            public string nodeInfo { get; set; }
        }
    }
}