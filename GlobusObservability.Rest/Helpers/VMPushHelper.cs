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
 
                                    foreach (var ms in measure.Value)
                                    {
                                        var networks = model.SubNetworks.Aggregate("", (current, net) => current + (net + "-"));

                                        var promMetric = Metrics.CreateGauge(measure.Key, "", new[]
                                        {
                                            $"{metric.Date:yyyy-MM-dd-HH-mm-ss}-{metric.NodeName}-{networks}",
                                            $"{metric.Date:yyyy-MM-dd-HH-mm-ss}",
                                            $"{networks}",
                                            $"{metric.NodeName}",
                                            $"{model.Id}",
                                            $"{measure.Key}"
                                        });

                                    var vmModel = new VmModel()
                                    {

                                    };


                                        //await Push(metricLabels, metricId, metricValue);
                                    }
                                }
                            }
                        }
                    }
                }
        }

        private class VmModel
        {
            public VmMetric metric { get; set; }
            public List<string> values { get; set; }
            public List<DateTime> timestamps { get; set; }
        }

        private class VmMetric
        {
            public string __name__ { get; set; }
            public string job { get; set; }
            public string instance { get; set; }
            public DateTime time { get; set; }
            public string[] subNetoworks { get; set; }
            public string nodeName { get; set; }
            public string measureId { get; set; }
        }
    }
}