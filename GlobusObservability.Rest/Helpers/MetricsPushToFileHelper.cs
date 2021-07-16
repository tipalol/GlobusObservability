using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GlobusObservability.Core.Entities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace GlobusObservability.Rest.Helpers
{
    public static class MetricsPushToFileHelper
    {
        public static IEnumerable<string> Push(IEnumerable<JsonMetricsModel> metrics, IConfiguration config)
        {
            var toPath = config.GetSection("Parsing")["MetricsJsonDestination"];

            var dir = Directory.CreateDirectory($"{toPath}{DateTime.Now:yyyy-MM-dd-HH-mm-ss}");

            var paths = new List<string>();

            foreach (var metric in metrics)
            {
                var subnetworks = metric.SubNetworks.Aggregate("", (current, subnet) => current + subnet);
                var newPath = $"{dir.FullName}/{metric.Date:yyyy-MM-dd-HH-mm-ss}-{metric.NodeName}-{subnetworks}.json";
                paths.Add(newPath);
                File.WriteAllText(newPath, JsonConvert.SerializeObject(metric, Formatting.Indented));
            }

            return paths;
        }
    }
}