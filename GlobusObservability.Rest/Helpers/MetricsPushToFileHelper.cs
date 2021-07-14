using System;
using System.Collections.Generic;
using System.IO;
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

            var dir = Directory.CreateDirectory($"{toPath}{DateTime.Today:u}");

            var paths = new List<string>();

            foreach (var metric in metrics)
            {
                var newPath = $"{dir.FullName}/{metric.Name}.json";
                paths.Add(newPath);
                File.WriteAllText(newPath, JsonConvert.SerializeObject(metric, Formatting.Indented));
            }

            return paths;
        }
    }
}