using System;
using System.Net.Http;
using System.Threading.Tasks;
using GlobusObservability.Core.Entities;
using Newtonsoft.Json;

namespace GlobusObservability.Rest.Helpers
{
    public class VmPushHelper
    {
        private const string Uri = "http://localhost:8428/api/v1/import";
        
        public async Task Push(JsonMetricsModel model)
        {
            var metric = new VmMetric() {Metric = model};

            var client = new HttpClient();

            await client.PostAsync(Uri, new StringContent(JsonConvert.SerializeObject(metric)));
        }

        private class VmMetric
        {
            public JsonMetricsModel Metric { get; set; }
        }
    }
}